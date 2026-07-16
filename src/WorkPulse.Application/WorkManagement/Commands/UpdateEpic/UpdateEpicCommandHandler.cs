using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.WorkManagement.Commands.UpdateEpic;

public sealed class UpdateEpicCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<UpdateEpicCommand, Result<EpicDto>>
{
	public async Task<Result<EpicDto>> Handle(UpdateEpicCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Epic epic = await context.Epics.FirstOrDefaultAsync((Epic e) => e.Id == request.EpicId, ct);
		if (epic == null)
		{
			return Error.NotFound("Epic.NotFound", "Epic not found.");
		}
		epic.Title = request.Title.Trim();
		epic.Description = request.Description?.Trim();
		epic.Status = request.Status;
		return new EpicDto(epic.Id, epic.TeamId, epic.Title, epic.Description, epic.Status.ToString(), epic.CreatedAtUtc);
	}
}
