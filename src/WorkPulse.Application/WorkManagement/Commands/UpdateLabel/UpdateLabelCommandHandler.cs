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

namespace WorkPulse.Application.WorkManagement.Commands.UpdateLabel;

public sealed class UpdateLabelCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<UpdateLabelCommand, Result<LabelDto>>
{
	public async Task<Result<LabelDto>> Handle(UpdateLabelCommand request, CancellationToken ct)
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
		string name = request.Name.Trim();
		if (await context.Labels.AnyAsync((Label l) => l.Name == name && l.Id != request.LabelId, ct))
		{
			return Error.Conflict("Label.NameTaken", "Label '" + name + "' already exists.");
		}
		label.Name = name;
		label.Color = request.Color.Trim();
		return new LabelDto(label.Id, label.Name, label.Color, label.CreatedAtUtc);
	}
}
