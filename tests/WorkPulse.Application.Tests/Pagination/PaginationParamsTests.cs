using WorkPulse.Application.Common.Pagination;

namespace WorkPulse.Application.Tests.Pagination;

/// <summary>
/// MaxPageSize was declared but never enforced anywhere, so <c>?pageSize=10000000</c> reached EF
/// unchanged and <c>?pageSize=0</c> divided by zero in PagedList.TotalPages.
/// </summary>
public sealed class PaginationParamsTests
{
	[Fact]
	public void Defaults_are_applied_when_nothing_is_supplied()
	{
		var p = new PaginationParams();

		Assert.Equal(PaginationParams.DefaultPage, p.Page);
		Assert.Equal(PaginationParams.DefaultPageSize, p.PageSize);
		Assert.Equal(0, p.Skip);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(25)]
	[InlineData(100)]
	public void A_page_size_within_range_is_preserved(int requested)
	{
		Assert.Equal(requested, new PaginationParams { PageSize = requested }.PageSize);
	}

	[Theory]
	[InlineData(101)]
	[InlineData(1_000)]
	[InlineData(10_000_000)]
	[InlineData(int.MaxValue)]
	public void An_oversized_page_size_is_clamped_to_the_maximum(int requested)
	{
		Assert.Equal(PaginationParams.MaxPageSize, new PaginationParams { PageSize = requested }.PageSize);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(int.MinValue)]
	public void A_non_positive_page_size_falls_back_to_the_default(int requested)
	{
		Assert.Equal(PaginationParams.DefaultPageSize, new PaginationParams { PageSize = requested }.PageSize);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-5)]
	[InlineData(int.MinValue)]
	public void A_non_positive_page_falls_back_to_the_first_page(int requested)
	{
		var p = new PaginationParams { Page = requested };

		Assert.Equal(PaginationParams.DefaultPage, p.Page);
		Assert.Equal(0, p.Skip);
	}

	[Fact]
	public void Skip_never_goes_negative()
	{
		Assert.True(new PaginationParams { Page = int.MinValue, PageSize = 50 }.Skip >= 0);
	}

	[Fact]
	public void Skip_is_computed_from_the_clamped_values()
	{
		// Page 3 with an over-max request: 100 per page, so skip 200 rather than overflowing.
		var p = new PaginationParams { Page = 3, PageSize = 5_000 };

		Assert.Equal(200, p.Skip);
	}
}
