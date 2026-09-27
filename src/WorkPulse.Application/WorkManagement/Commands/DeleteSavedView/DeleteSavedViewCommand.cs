using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.WorkManagement.Commands.DeleteSavedView;

public sealed record DeleteSavedViewCommand(Guid SavedViewId) : IRequest<Result>, IBaseRequest, ICommand;
