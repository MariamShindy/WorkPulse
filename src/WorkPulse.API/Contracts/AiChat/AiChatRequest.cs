using System.Collections.Generic;
using WorkPulse.Application.AiChat.Dtos;

namespace WorkPulse.API.Contracts.AiChat;

/// <summary>Request body for the assistant. Messages is the full conversation so far.</summary>
public sealed record AiChatRequest(List<ChatMessageDto> Messages);
