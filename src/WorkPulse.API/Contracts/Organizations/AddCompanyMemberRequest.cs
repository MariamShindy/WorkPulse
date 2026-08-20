using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Organizations;

public sealed record AddCompanyMemberRequest(Guid UserId, CompanyMemberRole Role);
