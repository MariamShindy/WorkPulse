using System.IO;

namespace WorkPulse.Application.Files.Services;

/// <summary>
/// Confirms that an upload's bytes match the content type it claims.
/// <para>
/// <c>IFormFile.ContentType</c> is just the browser-supplied <c>Content-Type</c> header and is
/// trivially spoofed, so the allow-list check alone lets an attacker store arbitrary bytes under
/// a permitted type. Sniffing the leading bytes closes that gap.
/// </para>
/// </summary>
public static class FileSignatureValidator
{
	/// <summary>Longest signature we compare, plus room for the WebP form (12 bytes).</summary>
	private const int HeaderBytes = 16;

	private static readonly byte[] Pdf = "%PDF"u8.ToArray();
	private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
	private static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF];
	private static readonly byte[] Gif87 = "GIF87a"u8.ToArray();
	private static readonly byte[] Gif89 = "GIF89a"u8.ToArray();
	private static readonly byte[] Riff = "RIFF"u8.ToArray();
	private static readonly byte[] Webp = "WEBP"u8.ToArray();

	// All OOXML formats (xlsx/docx) and plain archives are ZIP containers.
	private static readonly byte[] ZipLocal = [0x50, 0x4B, 0x03, 0x04];
	private static readonly byte[] ZipEmpty = [0x50, 0x4B, 0x05, 0x06];
	private static readonly byte[] ZipSpanned = [0x50, 0x4B, 0x07, 0x08];

	/// <summary>
	/// Reads the head of <paramref name="content"/> and rewinds it. Returns false when the bytes
	/// contradict <paramref name="declaredContentType"/>.
	/// </summary>
	public static async Task<bool> MatchesDeclaredTypeAsync(
		Stream content,
		string declaredContentType,
		CancellationToken ct = default)
	{
		if (!content.CanSeek)
		{
			// Without seek support we cannot inspect and then hand the full stream to storage.
			// Treat that as unverifiable rather than silently trusting the client.
			return false;
		}

		long origin = content.Position;
		byte[] buffer = new byte[HeaderBytes];
		int read = await content.ReadAtLeastAsync(buffer, HeaderBytes, throwOnEndOfStream: false, ct);
		content.Position = origin;

		ReadOnlySpan<byte> header = buffer.AsSpan(0, read);

		return declaredContentType.ToLowerInvariant() switch
		{
			"application/pdf" => header.StartsWith(Pdf),
			"image/png" => header.StartsWith(Png),
			"image/jpeg" => header.StartsWith(Jpeg),
			"image/gif" => header.StartsWith(Gif87) || header.StartsWith(Gif89),
			"image/webp" => IsWebp(header),
			"application/zip"
				or "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
				or "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
				=> header.StartsWith(ZipLocal) || header.StartsWith(ZipEmpty) || header.StartsWith(ZipSpanned),

			// Text formats carry no signature. Rejecting NUL bytes is what stops a renamed
			// binary being stored as text/csv; it does not reject any legitimate text file.
			"text/plain" or "text/csv" => !header.Contains((byte)0x00),

			// Unknown type: the allow-list check upstream already rejected it.
			_ => false
		};
	}

	/// <summary>WebP is <c>RIFF</c>, a 4-byte length, then <c>WEBP</c>.</summary>
	private static bool IsWebp(ReadOnlySpan<byte> header)
	{
		return header.Length >= 12
		       && header.StartsWith(Riff)
		       && header[8..12].SequenceEqual(Webp);
	}
}
