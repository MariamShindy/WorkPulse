using System.Text.Json.Serialization;

namespace WorkPulse.Infrastructure.Services.Ai;

internal sealed class OllamaChatResponse
{
	[JsonPropertyName("message")]
	public OllamaMessage? Message { get; set; }
}
