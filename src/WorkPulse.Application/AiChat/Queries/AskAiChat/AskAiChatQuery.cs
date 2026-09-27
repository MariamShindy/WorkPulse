using WorkPulse.Application.AiChat.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.AiChat.Queries.AskAiChat;

public sealed record AskAiChatQuery(IReadOnlyList<ChatMessageDto> Messages)
    : IRequest<Result<AiChatResponseDto>>, IBaseRequest, IQuery;
