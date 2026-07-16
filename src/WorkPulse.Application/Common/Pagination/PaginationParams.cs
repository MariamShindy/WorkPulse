namespace WorkPulse.Application.Common.Pagination;

public sealed record PaginationParams
{
	public int Page { get; init; } = 1;

	public int PageSize { get; init; } = 25;

	public int Skip => (Page - 1) * PageSize;

	public const int DefaultPage = 1;

	public const int DefaultPageSize = 25;

	public const int MaxPageSize = 100;
}
