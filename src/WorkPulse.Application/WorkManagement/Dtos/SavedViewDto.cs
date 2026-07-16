using System;

namespace WorkPulse.Application.WorkManagement.Dtos;

public sealed record SavedViewDto(Guid Id, Guid UserId, string Name, string EntityType, string FiltersJson, string SortJson, bool IsShared, DateTime CreatedAtUtc);
