using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Files.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Files.Queries;

public sealed record ListFilesQuery(FileEntityType EntityType, Guid EntityId, PaginationParams Pagination) : IRequest<Result<PagedList<StoredFileDto>>>, IBaseRequest, IQuery;
