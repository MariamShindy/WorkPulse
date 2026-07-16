using System;

namespace WorkPulse.Application.Projects.Dtos;

public sealed record WorkflowStateDto(Guid Id, string Name, string Type, string Color, int Position, bool IsDefault);
