using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Commands.AddCompanyMember;

public sealed class AddCompanyMemberCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<AddCompanyMemberCommand, Result<CompanyMemberDto>>
{
	public async Task<Result<CompanyMemberDto>> Handle(AddCompanyMemberCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		if (await context.CompanyMembers.AnyAsync((CompanyMember m) => m.UserId == request.UserId && m.IsActive, ct))
		{
			return Error.Conflict("Company.MemberExists", "User is already a member of this company.");
		}
		CompanyMember member = new CompanyMember
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			UserId = request.UserId,
			Role = request.Role,
			IsActive = true
		};
		context.CompanyMembers.Add(member);
		return new CompanyMemberDto(member.Id, member.UserId, string.Empty, string.Empty, member.Role.ToString(), member.IsActive, member.CreatedAtUtc);
	}
}
