using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Auth;

public sealed record InviteUserRequest(string Email, CompanyMemberRole Role);
