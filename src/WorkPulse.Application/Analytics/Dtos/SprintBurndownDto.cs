
namespace WorkPulse.Application.Analytics.Dtos;

public sealed record SprintBurndownDto(Guid SprintId, string SprintName, DateOnly StartDate, DateOnly EndDate, string Unit, double TotalScope, IReadOnlyList<BurndownPointDto> Points);
