
namespace WorkPulse.Application.Projects.Commands.RemoveTaskDependency;

public sealed class RemoveTaskDependencyCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<RemoveTaskDependencyCommand, Result>
{
	public async Task<Result> Handle(RemoveTaskDependencyCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		TaskDependency? dependency = await context.TaskDependencies.FirstOrDefaultAsync((TaskDependency d) => d.Id == request.DependencyId, ct);
		if (dependency is null)
		{
			return Error.NotFound("Task.DependencyNotFound", "Dependency not found.");
		}
		context.TaskDependencies.Remove(dependency);
		return Result.Success();
	}
}
