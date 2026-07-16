using System.Collections.Generic;
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

namespace WorkPulse.Application.WorkManagement.Commands.DeleteLabel;

public sealed class DeleteLabelCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<DeleteLabelCommand, Result>
{
	public async Task<Result> Handle(DeleteLabelCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Label label = await context.Labels.FirstOrDefaultAsync((Label l) => l.Id == request.LabelId, ct);
		if (label == null)
		{
			return Error.NotFound("Label.NotFound", "Label not found.");
		}
		List<TaskLabel> taskLabels = await context.TaskLabels.Where((TaskLabel tl) => tl.LabelId == request.LabelId).ToListAsync(ct);
		context.TaskLabels.RemoveRange(taskLabels);
		context.Labels.Remove(label);
		return Result.Success();
	}
}
