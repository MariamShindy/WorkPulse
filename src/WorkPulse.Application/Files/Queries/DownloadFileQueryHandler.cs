using WorkPulse.Application.Files.Dtos;

namespace WorkPulse.Application.Files.Queries;

public sealed class DownloadFileQueryHandler(IApplicationDbContext context, IFileStorageService storage, ITenantContext tenantContext) : IRequestHandler<DownloadFileQuery, Result<DownloadFileResult>>
{
	public async Task<Result<DownloadFileResult>> Handle(DownloadFileQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		StoredFile? file = await context.StoredFiles.AsNoTracking().FirstOrDefaultAsync((StoredFile f) => f.Id == request.FileId, ct);
		if (file is null)
		{
			return Error.NotFound("Files.NotFound", "File not found.");
		}
		return new DownloadFileResult(await storage.DownloadAsync(file.StorageKey, ct), file.FileName, file.ContentType);
	}
}
