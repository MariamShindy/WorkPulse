using System;

namespace WorkPulse.Application.Common.Pagination;

public sealed record SortParams
{
	public string? SortBy { get; init; }

	public string SortDirection { get; init; } = "asc";

	public bool IsDescending => SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
}
