using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Contracts.AiChat;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.AiChat.Dtos;
using WorkPulse.Application.AiChat.Queries.AskAiChat;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class AiController(ISender sender) : ControllerBase
{
    /// <summary>Ask the workspace AI assistant a question. Returns its answer plus which data tools it used.</summary>
    [HttpPost("chat")]
    public async Task<ActionResult<AiChatResponseDto>> Chat([FromBody] AiChatRequest request, CancellationToken ct)
    {
        IReadOnlyList<ChatMessageDto> messages = request.Messages ?? new List<ChatMessageDto>();
        return (await sender.Send((IRequest<Result<AiChatResponseDto>>)new AskAiChatQuery(messages), ct))
            .ToActionResult();
    }
}
