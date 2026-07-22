using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.AiChat.Dtos;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.AiChat.Queries.AskAiChat;

public sealed class AskAiChatQueryHandler(IAiChatService aiChat, ITenantContext tenantContext)
    : IRequestHandler<AskAiChatQuery, Result<AiChatResponseDto>>
{
    public async Task<Result<AiChatResponseDto>> Handle(AskAiChatQuery request, CancellationToken ct)
    {
        Result tenantCheck = tenantContext.EnsureResolved();
        if (tenantCheck.IsFailure)
        {
            return tenantCheck.Error;
        }

        if (request.Messages is null || request.Messages.Count == 0)
        {
            return Error.Validation("AiChat.EmptyConversation", "At least one message is required.");
        }

        return await aiChat.AskAsync(request.Messages, tenantContext.TenantId, ct);
    }
}
