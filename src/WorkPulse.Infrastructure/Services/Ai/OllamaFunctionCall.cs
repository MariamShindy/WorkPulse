using System.Text.Json;
using System.Text.Json.Serialization;

namespace WorkPulse.Infrastructure.Services.Ai;

internal sealed class OllamaFunctionCall
{
	[JsonPropertyName("name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("arguments")]
	public JsonElement? Arguments { get; set; }
}
