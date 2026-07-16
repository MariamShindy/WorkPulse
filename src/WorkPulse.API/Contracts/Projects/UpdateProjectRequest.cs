using System;
using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Projects;

public sealed record UpdateProjectRequest(string Name, string? Description, ProjectStatus Status, Guid? LeadId, DateOnly? StartDate, DateOnly? TargetDate);
