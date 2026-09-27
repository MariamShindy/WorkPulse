using WorkPulse.Application.Behaviors;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Commands.UpdateSavedView;

public sealed record UpdateSavedViewCommand(Guid SavedViewId, string Name, SavedViewEntityType EntityType, string FiltersJson, string SortJson, bool IsShared) : IRequest<Result<SavedViewDto>>, IBaseRequest, ICommand;
