using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Queries.ListMyCompanies;

public sealed record ListMyCompaniesQuery : IRequest<Result<IReadOnlyList<UserCompanyDto>>>, IBaseRequest, IQuery;
