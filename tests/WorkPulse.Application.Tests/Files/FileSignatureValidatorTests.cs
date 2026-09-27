using System.Text;
using WorkPulse.Application.Files.Services;

namespace WorkPulse.Application.Tests.Files;

/// <summary>
/// Uploads were accepted purely on the browser-supplied Content-Type, so a caller could store
/// arbitrary bytes under any allow-listed type. These cover the byte-level check that closes it.
/// </summary>
public sealed class FileSignatureValidatorTests
{
	private static MemoryStream Stream(params byte[] bytes) => new(bytes);

	private static MemoryStream Text(string value) => new(Encoding.UTF8.GetBytes(value));

	private static readonly byte[] PngHeader = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00];
	private static readonly byte[] JpegHeader = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10];
	private static readonly byte[] ZipHeader = [0x50, 0x4B, 0x03, 0x04, 0x14, 0x00];

	[Fact]
	public async Task A_real_png_is_accepted_as_png()
	{
		Assert.True(await FileSignatureValidator.MatchesDeclaredTypeAsync(Stream(PngHeader), "image/png"));
	}

	[Fact]
	public async Task A_real_jpeg_is_accepted_as_jpeg()
	{
		Assert.True(await FileSignatureValidator.MatchesDeclaredTypeAsync(Stream(JpegHeader), "image/jpeg"));
	}

	[Fact]
	public async Task A_pdf_is_accepted_as_pdf()
	{
		Assert.True(await FileSignatureValidator.MatchesDeclaredTypeAsync(Text("%PDF-1.7\n%..."), "application/pdf"));
	}

	[Fact]
	public async Task Ooxml_documents_are_accepted_as_zip_containers()
	{
		Assert.True(await FileSignatureValidator.MatchesDeclaredTypeAsync(
			Stream(ZipHeader),
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

		Assert.True(await FileSignatureValidator.MatchesDeclaredTypeAsync(
			Stream(ZipHeader),
			"application/vnd.openxmlformats-officedocument.wordprocessingml.document"));
	}

	[Fact]
	public async Task A_webp_needs_both_the_riff_and_webp_markers()
	{
		byte[] webp = [.. "RIFF"u8, 0x00, 0x00, 0x00, 0x00, .. "WEBP"u8, 0x56, 0x50];
		Assert.True(await FileSignatureValidator.MatchesDeclaredTypeAsync(Stream(webp), "image/webp"));

		// RIFF alone is also WAV/AVI, so it must not pass as an image.
		byte[] riffOnly = [.. "RIFF"u8, 0x00, 0x00, 0x00, 0x00, .. "WAVE"u8, 0x00, 0x00];
		Assert.False(await FileSignatureValidator.MatchesDeclaredTypeAsync(Stream(riffOnly), "image/webp"));
	}

	[Fact]
	public async Task Html_disguised_as_a_png_is_rejected()
	{
		// The attack this check exists for: served inline by /preview, this would have executed.
		MemoryStream payload = Text("<html><script>alert(document.cookie)</script></html>");

		Assert.False(await FileSignatureValidator.MatchesDeclaredTypeAsync(payload, "image/png"));
	}

	[Fact]
	public async Task An_svg_disguised_as_a_png_is_rejected()
	{
		MemoryStream payload = Text("<svg xmlns=\"http://www.w3.org/2000/svg\"><script>alert(1)</script></svg>");

		Assert.False(await FileSignatureValidator.MatchesDeclaredTypeAsync(payload, "image/png"));
	}

	[Fact]
	public async Task A_windows_executable_disguised_as_a_pdf_is_rejected()
	{
		byte[] pe = [0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00];

		Assert.False(await FileSignatureValidator.MatchesDeclaredTypeAsync(Stream(pe), "application/pdf"));
	}

	[Fact]
	public async Task A_binary_disguised_as_csv_is_rejected_via_its_null_bytes()
	{
		byte[] binary = [0x4D, 0x5A, 0x00, 0x00, 0x03, 0x00];

		Assert.False(await FileSignatureValidator.MatchesDeclaredTypeAsync(Stream(binary), "text/csv"));
	}

	[Fact]
	public async Task Genuine_text_and_csv_are_accepted()
	{
		Assert.True(await FileSignatureValidator.MatchesDeclaredTypeAsync(Text("id,name\n1,Alice\n"), "text/csv"));
		Assert.True(await FileSignatureValidator.MatchesDeclaredTypeAsync(Text("a release note"), "text/plain"));
	}

	[Fact]
	public async Task A_type_outside_the_allow_list_is_rejected()
	{
		Assert.False(await FileSignatureValidator.MatchesDeclaredTypeAsync(Stream(PngHeader), "image/svg+xml"));
		Assert.False(await FileSignatureValidator.MatchesDeclaredTypeAsync(Stream(PngHeader), "text/html"));
	}

	[Fact]
	public async Task The_stream_is_rewound_so_the_full_file_still_reaches_storage()
	{
		MemoryStream content = Stream([.. PngHeader, .. new byte[64]]);

		await FileSignatureValidator.MatchesDeclaredTypeAsync(content, "image/png");

		Assert.Equal(0, content.Position);
	}

	[Fact]
	public async Task A_file_shorter_than_its_signature_is_rejected()
	{
		Assert.False(await FileSignatureValidator.MatchesDeclaredTypeAsync(Stream(0x89, 0x50), "image/png"));
	}

	[Fact]
	public async Task An_empty_file_is_rejected()
	{
		Assert.False(await FileSignatureValidator.MatchesDeclaredTypeAsync(new MemoryStream(), "image/png"));
	}

	[Fact]
	public async Task A_non_seekable_stream_is_treated_as_unverifiable()
	{
		Assert.False(await FileSignatureValidator.MatchesDeclaredTypeAsync(
			new NonSeekableStream(PngHeader), "image/png"));
	}

	private sealed class NonSeekableStream(byte[] data) : MemoryStream(data)
	{
		public override bool CanSeek => false;
	}
}
