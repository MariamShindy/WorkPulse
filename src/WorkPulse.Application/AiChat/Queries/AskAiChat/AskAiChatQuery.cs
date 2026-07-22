using System.Collections.Generic;
using MediatR;
using WorkPulse.Application.AiChat.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.AiChat.Queries.AskAiChat;

public sealed record AskAiChatQuery(IReadOnlyList<ChatMessageDto> Messages)
    : IRequest<Result<AiChatResponseDto>>, IBaseRequest, IQuery;
