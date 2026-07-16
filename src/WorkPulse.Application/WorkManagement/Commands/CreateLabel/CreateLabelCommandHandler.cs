using System;
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

namespace WorkPulse.Application.WorkManagement.Commands.CreateLabel;

public sealed class CreateLabelCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<CreateLabelCommand, Result<LabelDto>>
{
	public async Task<Result<LabelDto>> Handle(CreateLabelCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		string name = request.Name.Trim();
		if (await context.Labels.AnyAsync((Label l) => l.Name == name, ct))
		{
			return Error.Conflict("Label.NameTaken", "Label '" + name + "' already exists.");
		}
		Label label = new Label
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			Name = name,
			Color = request.Color.Trim()
		};
		context.Labels.Add(label);
		return new LabelDto(label.Id, label.Name, label.Color, label.CreatedAtUtc);
	}
}
