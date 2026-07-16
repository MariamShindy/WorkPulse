using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.WorkManagement.Commands.DeleteEpic;

public sealed class DeleteEpicCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<DeleteEpicCommand, Result>
{
	public async Task<Result> Handle(DeleteEpicCommand request, CancellationToken ct)
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
		foreach (TaskItem task in await context.TaskItems.Where((TaskItem t) => t.EpicId == request.EpicId).ToListAsync(ct))
		{
			task.EpicId = null;
		}
		context.Epics.Remove(epic);
		return Result.Success();
	}
}
