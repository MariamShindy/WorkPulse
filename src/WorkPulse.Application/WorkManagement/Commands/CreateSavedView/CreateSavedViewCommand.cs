using WorkPulse.Application.Behaviors;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Commands.CreateSavedView;

public sealed record CreateSavedViewCommand(string Name, SavedViewEntityType EntityType, string FiltersJson, string SortJson, bool IsShared) : IRequest<Result<SavedViewDto>>, IBaseRequest, ICommand;
