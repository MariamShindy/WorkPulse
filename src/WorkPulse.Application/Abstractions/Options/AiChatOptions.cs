namespace WorkPulse.Application.Abstractions.Options;

/// <summary>
/// Configuration for the AI assistant, backed by a local Ollama server.
/// Bound from the "AiChat" section of appsettings.
/// </summary>
public sealed class AiChatOptions
{
    public const string SectionName = "AiChat";

    /// <summary>When false, the assistant endpoint returns a friendly "disabled" message.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Base URL of the Ollama server (e.g. http://localhost:11434).</summary>
    public string BaseUrl { get; set; } = "http://localhost:11434";

    /// <summary>Ollama model tag. Must be a tool-calling capable model (e.g. llama3.1, qwen2.5).</summary>
    public string Model { get; set; } = "llama3.1";

    /// <summary>Request timeout in seconds — local models can be slow on first token.</summary>
    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>Safety cap on the tool-call reasoning loop to avoid runaway calls.</summary>
    public int MaxToolIterations { get; set; } = 5;
}
