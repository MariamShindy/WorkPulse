using WorkPulse.Application.AiChat.Dtos;

namespace WorkPulse.Application.Abstractions;

/// <summary>
/// Conversational assistant over workspace data. Implementations run a
/// tool-calling loop against an LLM, executing insight tools scoped to the
/// given tenant, and return the model's final natural-language answer.
/// </summary>
public interface IAiChatService
{
    Task<Result<AiChatResponseDto>> AskAsync(
        IReadOnlyList<ChatMessageDto> messages,
        Guid tenantId,
        CancellationToken ct = default);
}
