using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Queries.GetCompany;

public sealed record GetCompanyQuery : IRequest<Result<CompanyDto>>, IBaseRequest, IQuery;
