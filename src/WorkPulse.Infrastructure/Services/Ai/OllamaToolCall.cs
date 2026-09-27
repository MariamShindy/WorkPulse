using System.Text.Json.Serialization;

namespace WorkPulse.Infrastructure.Services.Ai;

internal sealed class OllamaToolCall
{
	[JsonPropertyName("function")]
	public OllamaFunctionCall? Function { get; set; }
}
