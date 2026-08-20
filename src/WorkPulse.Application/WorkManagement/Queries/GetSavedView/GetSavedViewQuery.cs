using WorkPulse.Application.Behaviors;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.GetSavedView;

public sealed record GetSavedViewQuery(Guid SavedViewId) : IRequest<Result<SavedViewDto>>, IBaseRequest, IQuery;
