
namespace WorkPulse.Application.Collaboration.Dtos;

public sealed record TaskCommentDto(Guid Id, Guid TaskId, Guid AuthorId, string Body, IReadOnlyList<Guid> MentionedUserIds, bool IsEdited, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);
