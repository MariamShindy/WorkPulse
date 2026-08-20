namespace WorkPulse.Application.Abstractions;

public interface IFileStorageService
{
	Task<string> UploadAsync(Stream content, string fileName, string contentType, CancellationToken ct = default(CancellationToken));

	Task<Stream> DownloadAsync(string fileKey, CancellationToken ct = default(CancellationToken));

	Task DeleteAsync(string fileKey, CancellationToken ct = default(CancellationToken));

	string GetPublicUrl(string fileKey);
}
