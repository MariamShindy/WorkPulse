using System.IO;

namespace WorkPulse.Application.Files.Dtos;

public sealed record DownloadFileResult(Stream Content, string FileName, string ContentType);
