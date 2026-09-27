using WorkPulse.Application.Common.Pagination;

namespace WorkPulse.Application.Tests.Pagination;

public sealed class PagedListTests
{
	[Fact]
	public void TotalPages_rounds_up_a_partial_final_page()
	{
		var list = new PagedList<int>([1, 2, 3], page: 1, pageSize: 10, totalCount: 25);

		Assert.Equal(3, list.TotalPages);
	}

	[Fact]
	public void TotalPages_is_zero_rather_than_dividing_by_zero_when_page_size_is_zero()
	{
		var list = new PagedList<int>([], page: 1, pageSize: 0, totalCount: 25);

		Assert.Equal(0, list.TotalPages);
	}

	[Fact]
	public void Paging_flags_reflect_position_in_the_result_set()
	{
		var middle = new PagedList<int>([1], page: 2, pageSize: 10, totalCount: 25);

		Assert.True(middle.HasPreviousPage);
		Assert.True(middle.HasNextPage);

		var last = new PagedList<int>([1], page: 3, pageSize: 10, totalCount: 25);

		Assert.True(last.HasPreviousPage);
		Assert.False(last.HasNextPage);
	}

	[Fact]
	public void An_empty_result_set_has_no_next_page()
	{
		PagedList<int> empty = PagedList<int>.Empty(page: 1, pageSize: 25);

		Assert.Empty(empty.Items);
		Assert.Equal(0, empty.TotalPages);
		Assert.False(empty.HasNextPage);
		Assert.False(empty.HasPreviousPage);
	}
}
