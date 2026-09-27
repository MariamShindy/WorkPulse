using Microsoft.Extensions.Options;
using WorkPulse.Application.Abstractions.Options;
using WorkPulse.Application.Collaboration.Services;
using WorkPulse.Application.Files.Dtos;

using WorkPulse.Application.Files;
using WorkPulse.Application.Files.Services;

namespace WorkPulse.Application.Files.Commands.UploadFile;

public sealed class UploadFileCommandHandler(IApplicationDbContext context, IFileStorageService storage, ITenantContext tenantContext, ICurrentUserService currentUser, ITaskCollaborationService collaboration, IOptions<FileStorageOptions> options, IDateTime dateTime) : IRequestHandler<UploadFileCommand, Result<StoredFileDto>>
{
	public async Task<Result<StoredFileDto>> Handle(UploadFileCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
		{
			return Error.Unauthorized("Auth.Unauthorized", "Authentication is required.");
		}
		if (!options.Value.AllowedContentTypes.Contains<string>(request.ContentType, StringComparer.OrdinalIgnoreCase))
		{
			return Error.Validation("Files.InvalidType", "Content type '" + request.ContentType + "' is not allowed.");
		}
		// The declared type is client-controlled, so confirm the bytes agree with it before storing.
		if (!(await FileSignatureValidator.MatchesDeclaredTypeAsync(request.Content, request.ContentType, ct)))
		{
			return Error.Validation(
				"Files.ContentMismatch",
				"File contents do not match the declared type '" + request.ContentType + "'.");
		}
		if (!(await EntityExistsAsync(context, request.EntityType, request.EntityId, ct)))
		{
			return Error.NotFound("Files.EntityNotFound", "Linked entity not found.");
		}
		string storageKey = await storage.UploadAsync(request.Content, request.FileName, request.ContentType, ct);
		StoredFile file = new StoredFile
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			FileName = request.FileName,
			ContentType = request.ContentType,
			SizeBytes = request.SizeBytes,
			StorageKey = storageKey,
			UploadedById = currentUser.UserId.Value,
			EntityType = request.EntityType,
			EntityId = request.EntityId,
			CreatedAtUtc = dateTime.UtcNow
		};
		context.StoredFiles.Add(file);
		if (request.EntityType == FileEntityType.Task)
		{
			await collaboration.RecordActivityAsync(tenantContext.TenantId, request.EntityId, currentUser.UserId.Value, ActivityType.Attached, "Attached " + request.FileName, new
			{
				fileId = file.Id
			}, ct);
		}
		return new StoredFileDto(file.Id, file.FileName, file.ContentType, file.SizeBytes, FileUrls.Download(file.Id), file.EntityType.ToString(), file.EntityId, file.UploadedById, file.CreatedAtUtc);
	}

	private static Task<bool> EntityExistsAsync(IApplicationDbContext context, FileEntityType entityType, Guid entityId, CancellationToken ct)
	{
		if (1 == 0)
		{
		}
		Task<bool> result = entityType switch
		{
			FileEntityType.Task => context.TaskItems.AsNoTracking().AnyAsync((TaskItem t) => t.Id == entityId, ct), 
			FileEntityType.Project => context.Projects.AsNoTracking().AnyAsync((Project p) => p.Id == entityId, ct), 
			FileEntityType.Team => context.Teams.AsNoTracking().AnyAsync((Team t) => t.Id == entityId, ct), 
			FileEntityType.Comment => context.TaskComments.AsNoTracking().AnyAsync((TaskComment c) => c.Id == entityId, ct), 
			_ => Task.FromResult(result: false), 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
