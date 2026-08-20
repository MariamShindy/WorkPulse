using System.Text.Json.Serialization;

namespace WorkPulse.Infrastructure.Services.Ai;

internal sealed class OllamaParameters
{
	[JsonPropertyName("type")]
	public string Type { get; set; } = "object";

	[JsonPropertyName("properties")]
	public Dictionary<string, OllamaProperty> Properties { get; set; } = [];

	[JsonPropertyName("required")]
	public List<string> Required { get; set; } = [];
}
