using System.Text.Json.Serialization;

namespace WorkPulse.Infrastructure.Services.Ai;

internal sealed class OllamaTool
{
	[JsonPropertyName("type")]
	public string Type { get; set; } = "function";

	[JsonPropertyName("function")]
	public OllamaFunctionDef Function { get; set; } = new();
}
