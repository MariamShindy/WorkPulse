using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Files.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Files.Queries;

public sealed class ListFilesQueryHandler(IApplicationDbContext context, IFileStorageService storage, ITenantContext tenantContext) : IRequestHandler<ListFilesQuery, Result<PagedList<StoredFileDto>>>
{
	public async Task<Result<PagedList<StoredFileDto>>> Handle(ListFilesQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		IOrderedQueryable<StoredFile> query = context.StoredFiles.AsNoTracking().ForTenant(tenantContext)
			.Where((StoredFile f) => (int)f.EntityType == (int)request.EntityType && f.EntityId == request.EntityId)
			.OrderByDescending((StoredFile f) => f.CreatedAtUtc);
		int total = await query.CountAsync(ct);
		List<StoredFileDto> items = (await query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize).ToListAsync(ct)).Select((StoredFile f) => new StoredFileDto(f.Id, f.FileName, f.ContentType, f.SizeBytes, storage.GetPublicUrl(f.StorageKey), f.EntityType.ToString(), f.EntityId, f.UploadedById, f.CreatedAtUtc)).ToList();
		return new PagedList<StoredFileDto>(items, request.Pagination.Page, request.Pagination.PageSize, total);
	}
}
