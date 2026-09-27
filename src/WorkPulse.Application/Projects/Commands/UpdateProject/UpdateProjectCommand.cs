using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(Guid ProjectId, string Name, string? Description, ProjectStatus Status, Guid? LeadId, DateOnly? StartDate, DateOnly? TargetDate) : IRequest<Result<ProjectDto>>, IBaseRequest, ICommand;
