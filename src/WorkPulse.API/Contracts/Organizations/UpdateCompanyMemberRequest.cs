using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Organizations;

public sealed record UpdateCompanyMemberRequest(CompanyMemberRole Role, bool IsActive);
