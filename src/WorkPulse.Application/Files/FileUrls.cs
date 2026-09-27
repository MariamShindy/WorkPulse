namespace WorkPulse.Application.Files;

/// <summary>
/// Builds the API-relative links for a stored file. Kept next to the file handlers so the
/// route shape lives in one place — the previous storage-layer helper produced
/// <c>/api/files/download/{storageKey}</c>, which never matched the real controller route.
/// </summary>
public static class FileUrls
{
	public static string Download(Guid fileId) => $"/api/files/{fileId}/download";

	public static string Preview(Guid fileId) => $"/api/files/{fileId}/preview";
}
