using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(Guid TeamId, string Name, string? Key, string? Description, ProjectStatus Status, Guid? LeadId, DateOnly? StartDate, DateOnly? TargetDate) : IRequest<Result<ProjectDto>>, IBaseRequest, ICommand;
