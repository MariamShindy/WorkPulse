using System.Text.Json.Serialization;

namespace WorkPulse.Infrastructure.Services.Ai;

internal sealed class OllamaMessage
{
	[JsonPropertyName("role")]
	public string Role { get; set; } = "";

	[JsonPropertyName("content")]
	public string Content { get; set; } = "";

	[JsonPropertyName("tool_calls")]
	public List<OllamaToolCall>? ToolCalls { get; set; }
}
