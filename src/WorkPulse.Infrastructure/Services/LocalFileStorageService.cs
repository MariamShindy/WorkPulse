using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Options;

namespace WorkPulse.Infrastructure.Services;

public sealed class LocalFileStorageService(IOptions<FileStorageOptions> options) : IFileStorageService
{
	private readonly FileStorageOptions _options = options.Value;

	public async Task<string> UploadAsync(Stream content, string fileName, string contentType, CancellationToken ct = default(CancellationToken))
	{
		string storageKey = $"{Guid.NewGuid():N}_{SanitizeFileName(fileName)}";
		string fullPath = GetFullPath(storageKey);
		Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
		string result;
		await using (FileStream fileStream = File.Create(fullPath))
		{
			await content.CopyToAsync(fileStream, ct);
			result = storageKey;
		}
		return result;
	}

	public Task<Stream> DownloadAsync(string fileKey, CancellationToken ct = default(CancellationToken))
	{
		string fullPath = GetFullPath(fileKey);
		if (!File.Exists(fullPath))
		{
			throw new FileNotFoundException("Stored file not found.", fileKey);
		}
		Stream result = File.OpenRead(fullPath);
		return Task.FromResult(result);
	}

	public Task DeleteAsync(string fileKey, CancellationToken ct = default(CancellationToken))
	{
		string fullPath = GetFullPath(fileKey);
		if (File.Exists(fullPath))
		{
			File.Delete(fullPath);
		}
		return Task.CompletedTask;
	}

	public string GetPublicUrl(string fileKey)
	{
		return "/api/files/download/" + Uri.EscapeDataString(fileKey);
	}

	private string GetFullPath(string storageKey)
	{
		return Path.Combine(_options.RootPath, storageKey);
	}

	private static string SanitizeFileName(string fileName)
	{
		return Path.GetFileName(fileName).Replace("..", string.Empty, StringComparison.Ordinal);
	}
}
