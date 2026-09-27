using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Commands.UpdateCompany;

public sealed record UpdateCompanyCommand(string Name, string? Description, string? LogoUrl) : IRequest<Result<CompanyDto>>, IBaseRequest, ICommand;
