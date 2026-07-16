using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WorkPulse.Application.Common.Pagination;

public static class SortExtensions
{
	public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, SortParams? sort, IReadOnlyDictionary<string, Expression<Func<T, object>>> sortMap, Expression<Func<T, object>> defaultSort, bool defaultDescending = false)
	{
		if (sort?.SortBy != null && sortMap.TryGetValue(sort.SortBy, out Expression<Func<T, object>> value))
		{
			return sort.IsDescending ? query.OrderByDescending(value) : query.OrderBy(value);
		}
		return defaultDescending ? query.OrderByDescending(defaultSort) : query.OrderBy(defaultSort);
	}
}
