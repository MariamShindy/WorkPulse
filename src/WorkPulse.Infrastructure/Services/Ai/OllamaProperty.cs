using System.Text.Json.Serialization;

namespace WorkPulse.Infrastructure.Services.Ai;

internal sealed class OllamaProperty
{
	[JsonPropertyName("type")]
	public string Type { get; set; } = "";

	[JsonPropertyName("description")]
	public string Description { get; set; } = "";
}
