using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Commands.CreateEpic;

public sealed class CreateEpicCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<CreateEpicCommand, Result<EpicDto>>
{
	public async Task<Result<EpicDto>> Handle(CreateEpicCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		if (!(await context.Teams.AnyAsync((Team t) => t.Id == request.TeamId, ct)))
		{
			return Error.NotFound("Team.NotFound", "Team not found.");
		}
		Epic epic = new Epic
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			TeamId = request.TeamId,
			Title = request.Title.Trim(),
			Description = request.Description?.Trim(),
			Status = request.Status
		};
		context.Epics.Add(epic);
		return new EpicDto(epic.Id, epic.TeamId, epic.Title, epic.Description, epic.Status.ToString(), epic.CreatedAtUtc);
	}
}
