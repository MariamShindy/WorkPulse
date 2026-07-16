using System;
using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Projects;

public sealed record CreateProjectRequest(Guid TeamId, string Name, string? Key, string? Description, ProjectStatus Status, Guid? LeadId, DateOnly? StartDate, DateOnly? TargetDate);
