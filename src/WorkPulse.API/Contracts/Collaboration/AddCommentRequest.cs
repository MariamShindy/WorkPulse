using System;
using System.Collections.Generic;

namespace WorkPulse.API.Contracts.Collaboration;

public sealed record AddCommentRequest(string Body, IReadOnlyList<Guid>? MentionedUserIds);
