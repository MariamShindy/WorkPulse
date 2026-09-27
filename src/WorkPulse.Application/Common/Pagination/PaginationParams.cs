namespace WorkPulse.Application.Common.Pagination;

public sealed record PaginationParams
{
	public const int DefaultPage = 1;

	public const int DefaultPageSize = 25;

	public const int MaxPageSize = 100;

	private readonly int _page = DefaultPage;
	private readonly int _pageSize = DefaultPageSize;

	/// <summary>1-based page index. Values below 1 are clamped so callers cannot produce a negative skip.</summary>
	public int Page
	{
		get => _page;
		init => _page = value < 1 ? DefaultPage : value;
	}

	/// <summary>
	/// Rows per page, clamped to <see cref="MaxPageSize"/>. Clamping here rather than in each
	/// controller means an unbounded <c>?pageSize=</c> cannot force a full-table materialization,
	/// and a zero page size cannot divide by zero in <c>PagedList.TotalPages</c>.
	/// </summary>
	public int PageSize
	{
		get => _pageSize;
		init => _pageSize = value switch
		{
			< 1 => DefaultPageSize,
			> MaxPageSize => MaxPageSize,
			_ => value
		};
	}

	public int Skip => (Page - 1) * PageSize;
}
