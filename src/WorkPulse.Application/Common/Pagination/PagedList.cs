
namespace WorkPulse.Application.Common.Pagination;

public sealed class PagedList<T>
{
	public IReadOnlyList<T> Items { get; }

	public int Page { get; }

	public int PageSize { get; }

	public int TotalCount { get; }

	/// <summary>Guards against a zero page size from a directly-constructed instance.</summary>
	public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / (double)PageSize);

	public bool HasPreviousPage => Page > 1;

	public bool HasNextPage => Page < TotalPages;

	public PagedList(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
	{
		Items = items;
		Page = page;
		PageSize = pageSize;
		TotalCount = totalCount;
	}

	public static PagedList<T> Empty(int page, int pageSize)
	{
		return new PagedList<T>(Array.Empty<T>(), page, pageSize, 0);
	}

	public PagedList<TResult> Map<TResult>(Func<T, TResult> selector)
	{
		return new PagedList<TResult>(Items.Select(selector).ToList(), Page, PageSize, TotalCount);
	}
}
