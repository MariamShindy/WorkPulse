using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Files.Dtos;

namespace WorkPulse.Application.Files.Queries;

public sealed record ListFilesQuery(FileEntityType EntityType, Guid EntityId, PaginationParams Pagination) : IRequest<Result<PagedList<StoredFileDto>>>, IBaseRequest, IQuery;
