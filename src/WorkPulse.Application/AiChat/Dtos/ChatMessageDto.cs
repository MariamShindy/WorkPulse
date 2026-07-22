namespace WorkPulse.Application.AiChat.Dtos;

/// <summary>A single turn in the conversation. Role is "user" or "assistant".</summary>
public sealed record ChatMessageDto(string Role, string Content);
