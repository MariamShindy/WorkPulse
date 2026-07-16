using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Projects.Queries.ListProjects;

public sealed record ListProjectsQuery(PaginationParams Pagination, SortParams? Sort = null, Guid? TeamId = null, ProjectStatus? Status = null, bool IncludeArchived = false) : IRequest<Result<PagedList<ProjectDto>>>, IBaseRequest, IQuery;
