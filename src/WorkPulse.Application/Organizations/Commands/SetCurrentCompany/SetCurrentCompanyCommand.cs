using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Commands.SetCurrentCompany;

public sealed record SetCurrentCompanyCommand(Guid CompanyId) : IRequest<Result<CompanyDto>>, IBaseRequest, ICommand;
