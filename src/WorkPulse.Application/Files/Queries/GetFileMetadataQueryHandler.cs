using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Files.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Files.Queries;

public sealed class GetFileMetadataQueryHandler(IApplicationDbContext context, IFileStorageService storage, ITenantContext tenantContext) : IRequestHandler<GetFileMetadataQuery, Result<StoredFileDto>>
{
	public async Task<Result<StoredFileDto>> Handle(GetFileMetadataQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		StoredFile file = await context.StoredFiles.AsNoTracking().FirstOrDefaultAsync((StoredFile f) => f.Id == request.FileId, ct);
		if (file == null)
		{
			return Error.NotFound("Files.NotFound", "File not found.");
		}
		return new StoredFileDto(file.Id, file.FileName, file.ContentType, file.SizeBytes, storage.GetPublicUrl(file.StorageKey), file.EntityType.ToString(), file.EntityId, file.UploadedById, file.CreatedAtUtc);
	}
}
