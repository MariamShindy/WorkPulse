namespace WorkPulse.Application.AiChat.Dtos;

/// <summary>
/// The assistant's reply plus the names of any data tools it invoked
/// (surfaced in the UI so the answer is transparent, not a black box).
/// </summary>
public sealed record AiChatResponseDto(string Reply, IReadOnlyList<string> ToolsUsed);
