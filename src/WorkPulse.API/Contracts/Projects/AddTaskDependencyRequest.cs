using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Projects;

public sealed record AddTaskDependencyRequest(Guid DependsOnTaskId, TaskDependencyType Type);
