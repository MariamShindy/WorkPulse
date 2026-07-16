using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Files.Commands.DeleteFile;

public sealed class DeleteFileCommandHandler(IApplicationDbContext context, IFileStorageService storage, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<DeleteFileCommand, Result>
{
	public async Task<Result> Handle(DeleteFileCommand request, CancellationToken ct)
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
		StoredFile file = await context.StoredFiles.FirstOrDefaultAsync((StoredFile f) => f.Id == request.FileId, ct);
		if (file == null)
		{
			return Error.NotFound("Files.NotFound", "File not found.");
		}
		await storage.DeleteAsync(file.StorageKey, ct);
		context.StoredFiles.Remove(file);
		return Result.Success();
	}
}
