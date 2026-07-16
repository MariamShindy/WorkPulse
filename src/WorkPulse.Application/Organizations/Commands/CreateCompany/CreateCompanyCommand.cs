using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Commands.CreateCompany;

public sealed record CreateCompanyCommand(string Name, string? Description, string? LogoUrl) : IRequest<Result<CompanyDto>>, IBaseRequest, ICommand;
