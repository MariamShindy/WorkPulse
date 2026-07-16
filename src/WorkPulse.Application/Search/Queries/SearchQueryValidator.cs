using FluentValidation;

namespace WorkPulse.Application.Search.Queries;

public sealed class SearchQueryValidator : AbstractValidator<SearchQuery>
{
	public SearchQueryValidator()
	{
		RuleFor((SearchQuery x) => x.Query).NotEmpty().MinimumLength(2).MaximumLength(200);
		RuleFor((SearchQuery x) => x.Limit).InclusiveBetween(1, 100);
	}
}
