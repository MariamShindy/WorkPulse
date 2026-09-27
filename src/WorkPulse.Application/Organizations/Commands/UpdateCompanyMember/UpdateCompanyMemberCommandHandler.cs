
namespace WorkPulse.Application.Organizations.Commands.UpdateCompanyMember;

public sealed class UpdateCompanyMemberCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<UpdateCompanyMemberCommand, Result>
{
	public async Task<Result> Handle(UpdateCompanyMemberCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		CompanyMember? member = await context.CompanyMembers.FirstOrDefaultAsync((CompanyMember m) => m.Id == request.MemberId, ct);
		if (member is null)
		{
			return Error.NotFound("Company.MemberNotFound", "Company member not found.");
		}
		member.Role = request.Role;
		member.IsActive = request.IsActive;
		return Result.Success();
	}
}
