using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Files.Commands.DeleteFile;
using WorkPulse.Application.Files.Commands.UploadFile;
using WorkPulse.Application.Files.Dtos;
using WorkPulse.Application.Files.Queries;
using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/files")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class FilesController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<PagedList<StoredFileDto>>> List([FromQuery] FileEntityType entityType, [FromQuery] Guid entityId, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<StoredFileDto>>>)new ListFilesQuery(entityType, entityId, new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}), ct)).ToActionResult();
	}

	[HttpGet("{fileId:guid}")]
	public async Task<ActionResult<StoredFileDto>> GetMetadata(Guid fileId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<StoredFileDto>>)new GetFileMetadataQuery(fileId), ct)).ToActionResult();
	}

	[HttpPost("upload")]
	[RequestSizeLimit(10485760L)]
	public async Task<ActionResult<StoredFileDto>> Upload(IFormFile file, [FromForm] FileEntityType entityType, [FromForm] Guid entityId, CancellationToken ct)
	{
		ActionResult<StoredFileDto> result2;
		await using (Stream stream = file.OpenReadStream())
		{
			Result<StoredFileDto> result = await sender.Send((IRequest<Result<StoredFileDto>>)new UploadFileCommand(stream, file.FileName, file.ContentType, file.Length, entityType, entityId), ct);
			result2 = ((!result.IsFailure) ? ((ActionResult<StoredFileDto>)CreatedAtAction("GetMetadata", new
			{
				fileId = result.Value.Id
			}, result.Value)) : result.ToActionResult());
		}
		return result2;
	}

	[HttpGet("{fileId:guid}/download")]
	public async Task<ActionResult> Download(Guid fileId, CancellationToken ct)
	{
		Result<DownloadFileResult> result = await sender.Send((IRequest<Result<DownloadFileResult>>)new DownloadFileQuery(fileId), ct);
		if (result.IsFailure)
		{
			return result.ToErrorActionResult();
		}
		return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
	}

	[HttpGet("{fileId:guid}/preview")]
	public async Task<ActionResult> Preview(Guid fileId, CancellationToken ct)
	{
		Result<DownloadFileResult> result = await sender.Send((IRequest<Result<DownloadFileResult>>)new DownloadFileQuery(fileId), ct);
		if (result.IsFailure)
		{
			return result.ToErrorActionResult();
		}
		if (!result.Value.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
		{
			return UnprocessableEntity(new ProblemDetails
			{
				Title = "Validation",
				Detail = "Preview is only available for image files."
			});
		}
		base.Response.Headers.ContentDisposition = "inline; filename=\"" + result.Value.FileName + "\"";
		return File(result.Value.Content, result.Value.ContentType);
	}

	[HttpDelete("{fileId:guid}")]
	public async Task<IActionResult> Delete(Guid fileId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new DeleteFileCommand(fileId), ct)).ToActionResult();
	}
}
