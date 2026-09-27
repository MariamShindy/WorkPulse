using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Files.Dtos;

using WorkPulse.Application.Files;

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
		List<StoredFileDto> items = (await query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize).ToListAsync(ct)).Select((StoredFile f) => new StoredFileDto(f.Id, f.FileName, f.ContentType, f.SizeBytes, FileUrls.Download(f.Id), f.EntityType.ToString(), f.EntityId, f.UploadedById, f.CreatedAtUtc)).ToList();
		return new PagedList<StoredFileDto>(items, request.Pagination.Page, request.Pagination.PageSize, total);
	}
}
