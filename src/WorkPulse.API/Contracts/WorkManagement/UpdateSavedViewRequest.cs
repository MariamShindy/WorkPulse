using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.WorkManagement;

public sealed record UpdateSavedViewRequest(string Name, SavedViewEntityType EntityType, string FiltersJson, string SortJson, bool IsShared);
