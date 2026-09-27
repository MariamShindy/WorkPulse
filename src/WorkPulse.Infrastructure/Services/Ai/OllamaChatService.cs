using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using WorkPulse.Application.Abstractions.Options;
using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.AiChat.Dtos;

namespace WorkPulse.Infrastructure.Services.Ai;

/// <summary>
/// AI assistant backed by a local Ollama server. Runs a tool-calling loop:
/// the model may request one of the workspace insight tools, we execute it
/// against the database (tenant-scoped), feed the JSON result back, and repeat
/// until the model produces a final natural-language answer.
/// </summary>
public sealed class OllamaChatService(
	HttpClient httpClient,
	IAiInsightsReadService insights,
	IOptions<AiChatOptions> options,
	ILogger<OllamaChatService> logger) : IAiChatService
{
	private readonly AiChatOptions _options = options.Value;

	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
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
		"FORMAT every answer in clean Markdown that is easy to scan: " +
		"start with one short summary sentence, then structure the details. " +
		"For lists of more than 5 items, prefer a Markdown table with clear columns " +
		"(e.g. Task | Owner | Days overdue | Priority) OR group under ## headings by person — " +
		"never dump a long flat bullet wall of 15+ similar lines. " +
		"Use **bold** for names and key numbers. Keep bullets short when you use them. " +
		"End with a single actionable suggestion when relevant. " +
		"If the data shows nothing (empty results), say so plainly rather than guessing.";

	public async Task<Result<AiChatResponseDto>> AskAsync(
		IReadOnlyList<ChatMessageDto> messages,
		Guid tenantId,
		CancellationToken cancellationToken = default)
	{
		if (!_options.Enabled)
		{
			return Result.Success(new AiChatResponseDto(
				"The AI assistant is currently disabled. Ask an administrator to enable it in configuration.",
				Array.Empty<string>()));
		}

		List<OllamaMessage> conversation = BuildConversation(messages);
		List<string> toolsUsed = [];

		try
		{
			return await RunToolLoopAsync(conversation, toolsUsed, tenantId, cancellationToken);
		}
		catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
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

	private static List<OllamaMessage> BuildConversation(IReadOnlyList<ChatMessageDto> messages)
	{
		List<OllamaMessage> conversation =
		[
			new() { Role = "system", Content = SystemPrompt }
		];

		conversation.AddRange(messages
			.Where(message => !string.IsNullOrWhiteSpace(message.Content))
			.Select(message => new OllamaMessage
			{
				Role = message.Role == "assistant" ? "assistant" : "user",
				Content = message.Content.Trim()
			}));

		return conversation;
	}

	private async Task<Result<AiChatResponseDto>> RunToolLoopAsync(
		List<OllamaMessage> conversation,
		List<string> toolsUsed,
		Guid tenantId,
		CancellationToken cancellationToken)
	{
		for (int iteration = 0; iteration < _options.MaxToolIterations; iteration++)
		{
			OllamaChatResponse response = await SendAsync(conversation, cancellationToken);
			OllamaMessage assistantMessage = response.Message ?? new OllamaMessage { Role = "assistant", Content = "" };

			if (assistantMessage.ToolCalls is null || assistantMessage.ToolCalls.Count == 0)
			{
				return Result.Success(BuildFinalResponse(assistantMessage.Content, toolsUsed));
			}

			await AppendToolResultsAsync(conversation, toolsUsed, assistantMessage, tenantId, cancellationToken);
		}

		return await SummarizeAfterToolCapAsync(conversation, toolsUsed, cancellationToken);
	}

	private static AiChatResponseDto BuildFinalResponse(string? content, IEnumerable<string> toolsUsed)
	{
		string reply = string.IsNullOrWhiteSpace(content)
			? "I couldn't produce an answer for that. Try rephrasing your question."
			: content.Trim();

		return new AiChatResponseDto(reply, toolsUsed.Distinct().ToList());
	}

	private async Task AppendToolResultsAsync(
		List<OllamaMessage> conversation,
		List<string> toolsUsed,
		OllamaMessage assistantMessage,
		Guid tenantId,
		CancellationToken cancellationToken)
	{
		conversation.Add(assistantMessage);

		foreach (OllamaToolCall toolCall in assistantMessage.ToolCalls!)
		{
			string toolName = toolCall.Function?.Name ?? "unknown";
			toolsUsed.Add(toolName);
			string toolResult = await ExecuteToolAsync(toolName, toolCall.Function?.Arguments, tenantId, cancellationToken);
			conversation.Add(new OllamaMessage { Role = "tool", Content = toolResult });
		}
	}

	private async Task<Result<AiChatResponseDto>> SummarizeAfterToolCapAsync(
		List<OllamaMessage> conversation,
		List<string> toolsUsed,
		CancellationToken cancellationToken)
	{
		logger.LogWarning("AI chat hit the max tool-iteration cap ({Max}).", _options.MaxToolIterations);

		OllamaChatResponse finalResponse = await SendAsync(conversation, cancellationToken, includeTools: false);
		string finalReply = finalResponse.Message?.Content?.Trim() ?? "";

		return Result.Success(new AiChatResponseDto(
			string.IsNullOrWhiteSpace(finalReply)
				? "I gathered the data but couldn't summarise it. Please try a more specific question."
				: finalReply,
			toolsUsed.Distinct().ToList()));
	}

	private async Task<OllamaChatResponse> SendAsync(
		List<OllamaMessage> conversation,
		CancellationToken cancellationToken,
		bool includeTools = true)
	{
		var request = new OllamaChatRequest
		{
			Model = _options.Model,
			Messages = conversation,
			Stream = false,
			Tools = includeTools ? ToolCatalog : null
		};

		using HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync("api/chat", request, JsonOptions, cancellationToken);
		httpResponse.EnsureSuccessStatusCode();

		OllamaChatResponse? response = await httpResponse.Content.ReadFromJsonAsync<OllamaChatResponse>(JsonOptions, cancellationToken);
		return response ?? new OllamaChatResponse();
	}

	private async Task<string> ExecuteToolAsync(
		string toolName,
		JsonElement? arguments,
		Guid tenantId,
		CancellationToken cancellationToken)
	{
		try
		{
			object payload = toolName switch
			{
				"get_member_workload" => await insights.GetMemberWorkloadAsync(tenantId, cancellationToken),
				"get_monthly_kpis" => await insights.GetMonthlyKpisAsync(tenantId, OllamaToolArgumentParser.ReadMonths(arguments), cancellationToken),
				"get_overdue_tasks" => await insights.GetOverdueTasksAsync(tenantId, cancellationToken),
				"get_workspace_summary" => await insights.GetWorkspaceSummaryAsync(tenantId, cancellationToken),
				_ => new { error = $"Unknown tool '{toolName}'." }
			};
			return JsonSerializer.Serialize(payload, JsonOptions);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "AI tool {Tool} failed.", toolName);
			return JsonSerializer.Serialize(new { error = "The tool failed to retrieve data." }, JsonOptions);
		}
	}

	private static readonly IReadOnlyList<OllamaTool> ToolCatalog =
	[
		CreateTool(
			"get_member_workload",
			"Returns every team member with their open, overdue and completed task counts. " +
			"Use for questions about who has the most tasks, workload balance, or who is overloaded."),
		CreateTool(
			"get_monthly_kpis",
			"Returns per-month KPIs (created, completed, overdue, average cycle time in days) for recent months. " +
			"Use for monthly trends, velocity, or 'how did we do this month'.",
			includeMonthsParameter: true),
		CreateTool(
			"get_overdue_tasks",
			"Returns all overdue tasks with the assignee name, due date and how many days late they are. " +
			"Use for questions about who is behind schedule or what is late."),
		CreateTool(
			"get_workspace_summary",
			"Returns high-level totals for the whole workspace: total/open/completed/overdue/unassigned tasks, " +
			"plus member, team and project counts. Use for a general overview."),
	];

	private static OllamaTool CreateTool(string name, string description, bool includeMonthsParameter = false)
	{
		Dictionary<string, OllamaProperty> properties = [];
		List<string> required = [];

		if (includeMonthsParameter)
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
}
