using System.Text.Json.Serialization;

namespace WorkPulse.Infrastructure.Services.Ai;

internal sealed class OllamaChatRequest
{
	[JsonPropertyName("model")]
	public string Model { get; set; } = "";

	[JsonPropertyName("messages")]
	public List<OllamaMessage> Messages { get; set; } = [];

	[JsonPropertyName("stream")]
	public bool Stream { get; set; }

	[JsonPropertyName("tools")]
	public IReadOnlyList<OllamaTool>? Tools { get; set; }
}
