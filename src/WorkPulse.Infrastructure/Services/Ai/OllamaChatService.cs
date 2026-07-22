using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Options;
using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.AiChat.Dtos;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Infrastructure.Services.Ai;

/// <summary>
/// AI assistant backed by a local Ollama server. Runs a tool-calling loop:
/// the model may request one of the workspace insight tools, we execute it
/// against the database (tenant-scoped), feed the JSON result back, and repeat
/// until the model produces a final natural-language answer.
/// </summary>
public sealed class OllamaChatService(
    HttpClient http,
    IAiInsightsReadService insights,
    IOptions<AiChatOptions> options,
    ILogger<OllamaChatService> logger) : IAiChatService
{
    private readonly AiChatOptions _options = options.Value;

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private const string SystemPrompt =
        "You are WorkPulse Assistant, a project-management analyst embedded in a team's workspace. " +
        "Answer questions about tasks, team members, workload, deadlines and monthly KPIs. " +
        "You MUST use the provided tools to obtain real numbers — never invent figures. " +
        "When a question is about who is busy, who is behind, or workload distribution, call get_member_workload. " +
        "For monthly trends or KPIs, call get_monthly_kpis. For late/overdue work, call get_overdue_tasks. " +
        "For a general overview, call get_workspace_summary. " +
        "After gathering data, give a concise, well-structured answer. Use short paragraphs or bullet points, " +
        "name people explicitly, call out anyone overloaded or behind schedule, and end with one actionable suggestion when relevant. " +
        "If the data shows nothing (empty results), say so plainly rather than guessing.";

    public async Task<Result<AiChatResponseDto>> AskAsync(
        IReadOnlyList<ChatMessageDto> messages,
        Guid tenantId,
        CancellationToken ct = default)
    {
        if (!_options.Enabled)
        {
            return Result.Success(new AiChatResponseDto(
                "The AI assistant is currently disabled. Ask an administrator to enable it in configuration.",
                Array.Empty<string>()));
        }

        // Build the running conversation, starting with the system prompt.
        var convo = new List<OllamaMessage>
        {
            new() { Role = "system", Content = SystemPrompt }
        };
        convo.AddRange(messages.Select(m => new OllamaMessage
        {
            Role = m.Role == "assistant" ? "assistant" : "user",
            Content = m.Content
        }));

        var toolsUsed = new List<string>();

        try
        {
            for (int iteration = 0; iteration < _options.MaxToolIterations; iteration++)
            {
                OllamaChatResponse response = await SendAsync(convo, ct);
                OllamaMessage assistant = response.Message ?? new OllamaMessage { Role = "assistant", Content = "" };

                // No tool calls → this is the final answer.
                if (assistant.ToolCalls is null || assistant.ToolCalls.Count == 0)
                {
                    string reply = string.IsNullOrWhiteSpace(assistant.Content)
                        ? "I couldn't produce an answer for that. Try rephrasing your question."
                        : assistant.Content.Trim();
                    return Result.Success(new AiChatResponseDto(reply, toolsUsed.Distinct().ToList()));
                }

                // Record the assistant's tool-call turn, then execute each tool.
                convo.Add(assistant);
                foreach (OllamaToolCall call in assistant.ToolCalls)
                {
                    string name = call.Function?.Name ?? "unknown";
                    toolsUsed.Add(name);
                    string toolResult = await ExecuteToolAsync(name, call.Function?.Arguments, tenantId, ct);
                    convo.Add(new OllamaMessage { Role = "tool", Content = toolResult });
                }
            }

            // Exhausted the loop — ask once more without tools for a plain summary.
            logger.LogWarning("AI chat hit the max tool-iteration cap ({Max}).", _options.MaxToolIterations);
            OllamaChatResponse finalResponse = await SendAsync(convo, ct, includeTools: false);
            string finalReply = finalResponse.Message?.Content?.Trim() ?? "";
            return Result.Success(new AiChatResponseDto(
                string.IsNullOrWhiteSpace(finalReply)
                    ? "I gathered the data but couldn't summarise it. Please try a more specific question."
                    : finalReply,
                toolsUsed.Distinct().ToList()));
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            logger.LogError("AI chat request to Ollama timed out after {Seconds}s.", _options.TimeoutSeconds);
            return Error.Failure("AiChat.Timeout",
                "The AI model took too long to respond. It may still be loading — please try again in a moment.");
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "AI chat could not reach the Ollama server at {BaseUrl}.", _options.BaseUrl);
            return Error.Failure("AiChat.Unavailable",
                "The AI assistant is unavailable. Ensure the Ollama server is running and the model is installed.");
        }
    }

    private async Task<OllamaChatResponse> SendAsync(List<OllamaMessage> convo, CancellationToken ct, bool includeTools = true)
    {
        var request = new OllamaChatRequest
        {
            Model = _options.Model,
            Messages = convo,
            Stream = false,
            Tools = includeTools ? ToolCatalog : null
        };

        using HttpResponseMessage resp = await http.PostAsJsonAsync("api/chat", request, Json, ct);
        resp.EnsureSuccessStatusCode();
        OllamaChatResponse? response = await resp.Content.ReadFromJsonAsync<OllamaChatResponse>(Json, ct);
        return response ?? new OllamaChatResponse();
    }

    private async Task<string> ExecuteToolAsync(string name, JsonElement? args, Guid tenantId, CancellationToken ct)
    {
        try
        {
            object payload = name switch
            {
                "get_member_workload" => await insights.GetMemberWorkloadAsync(tenantId, ct),
                "get_monthly_kpis" => await insights.GetMonthlyKpisAsync(tenantId, ReadMonths(args), ct),
                "get_overdue_tasks" => await insights.GetOverdueTasksAsync(tenantId, ct),
                "get_workspace_summary" => await insights.GetWorkspaceSummaryAsync(tenantId, ct),
                _ => new { error = $"Unknown tool '{name}'." }
            };
            return JsonSerializer.Serialize(payload, Json);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AI tool {Tool} failed.", name);
            return JsonSerializer.Serialize(new { error = "The tool failed to retrieve data." }, Json);
        }
    }

    private static int ReadMonths(JsonElement? args)
    {
        const int fallback = 6;
        if (args is not { ValueKind: JsonValueKind.Object } obj) return fallback;
        if (!obj.TryGetProperty("months", out JsonElement months)) return fallback;
        return months.ValueKind switch
        {
            JsonValueKind.Number when months.TryGetInt32(out int n) => n,
            JsonValueKind.String when int.TryParse(months.GetString(), out int n) => n,
            _ => fallback
        };
    }

    // ── Tool catalog (JSON-schema function definitions sent to the model) ────────

    private static readonly IReadOnlyList<OllamaTool> ToolCatalog =
    [
        Tool("get_member_workload",
            "Returns every team member with their open, overdue and completed task counts. " +
            "Use for questions about who has the most tasks, workload balance, or who is overloaded."),
        Tool("get_monthly_kpis",
            "Returns per-month KPIs (created, completed, overdue, average cycle time in days) for recent months. " +
            "Use for monthly trends, velocity, or 'how did we do this month'.",
            monthsParam: true),
        Tool("get_overdue_tasks",
            "Returns all overdue tasks with the assignee name, due date and how many days late they are. " +
            "Use for questions about who is behind schedule or what is late."),
        Tool("get_workspace_summary",
            "Returns high-level totals for the whole workspace: total/open/completed/overdue/unassigned tasks, " +
            "plus member, team and project counts. Use for a general overview."),
    ];

    private static OllamaTool Tool(string name, string description, bool monthsParam = false)
    {
        var properties = new Dictionary<string, OllamaProperty>();
        var required = new List<string>();
        if (monthsParam)
        {
            properties["months"] = new OllamaProperty
            {
                Type = "integer",
                Description = "How many recent months to include (1-24). Defaults to 6."
            };
        }
        return new OllamaTool
        {
            Type = "function",
            Function = new OllamaFunctionDef
            {
                Name = name,
                Description = description,
                Parameters = new OllamaParameters
                {
                    Type = "object",
                    Properties = properties,
                    Required = required
                }
            }
        };
    }

    // ── Ollama wire-format DTOs ──────────────────────────────────────────────────

    private sealed class OllamaChatRequest
    {
        [JsonPropertyName("model")] public string Model { get; set; } = "";
        [JsonPropertyName("messages")] public List<OllamaMessage> Messages { get; set; } = [];
        [JsonPropertyName("stream")] public bool Stream { get; set; }
        [JsonPropertyName("tools")] public IReadOnlyList<OllamaTool>? Tools { get; set; }
    }

    private sealed class OllamaChatResponse
    {
        [JsonPropertyName("message")] public OllamaMessage? Message { get; set; }
    }

    private sealed class OllamaMessage
    {
        [JsonPropertyName("role")] public string Role { get; set; } = "";
        [JsonPropertyName("content")] public string Content { get; set; } = "";
        [JsonPropertyName("tool_calls")] public List<OllamaToolCall>? ToolCalls { get; set; }
    }

    private sealed class OllamaToolCall
    {
        [JsonPropertyName("function")] public OllamaFunctionCall? Function { get; set; }
    }

    private sealed class OllamaFunctionCall
    {
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("arguments")] public JsonElement? Arguments { get; set; }
    }

    private sealed class OllamaTool
    {
        [JsonPropertyName("type")] public string Type { get; set; } = "function";
        [JsonPropertyName("function")] public OllamaFunctionDef Function { get; set; } = new();
    }

    private sealed class OllamaFunctionDef
    {
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("description")] public string Description { get; set; } = "";
        [JsonPropertyName("parameters")] public OllamaParameters Parameters { get; set; } = new();
    }

    private sealed class OllamaParameters
    {
        [JsonPropertyName("type")] public string Type { get; set; } = "object";
        [JsonPropertyName("properties")] public Dictionary<string, OllamaProperty> Properties { get; set; } = [];
        [JsonPropertyName("required")] public List<string> Required { get; set; } = [];
    }

    private sealed class OllamaProperty
    {
        [JsonPropertyName("type")] public string Type { get; set; } = "";
        [JsonPropertyName("description")] public string Description { get; set; } = "";
    }
}
