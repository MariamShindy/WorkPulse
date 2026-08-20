using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Organizations.Commands.UpdateCompanyMember;

public sealed record UpdateCompanyMemberCommand(Guid MemberId, CompanyMemberRole Role, bool IsActive) : IRequest<Result>, IBaseRequest, ICommand;
