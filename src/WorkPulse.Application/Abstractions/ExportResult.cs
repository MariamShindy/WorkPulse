namespace WorkPulse.Application.Abstractions;

public sealed record ExportResult(byte[] Content, string ContentType, string FileName);
