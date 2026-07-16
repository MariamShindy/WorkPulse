namespace WorkPulse.Application.Abstractions.Options;

public sealed class FileStorageOptions
{
	public const string SectionName = "FileStorage";

	public string RootPath { get; set; } = "uploads";

	public long MaxFileSizeBytes { get; set; } = 10485760L;

	public string[] AllowedContentTypes { get; set; } = new string[10] { "application/pdf", "image/png", "image/jpeg", "image/gif", "image/webp", "text/plain", "text/csv", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/zip" };
}
