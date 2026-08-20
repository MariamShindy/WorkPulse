using System.Text.Json.Serialization;

namespace WorkPulse.Infrastructure.Services.Ai;

internal sealed class OllamaFunctionDef
{
	[JsonPropertyName("name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("description")]
	public string Description { get; set; } = "";

	[JsonPropertyName("parameters")]
	public OllamaParameters Parameters { get; set; } = new();
}
