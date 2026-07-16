using System;

namespace WorkPulse.Application.Files.Dtos;

public sealed record StoredFileDto(Guid Id, string FileName, string ContentType, long SizeBytes, string PublicUrl, string EntityType, Guid EntityId, Guid UploadedById, DateTime CreatedAtUtc);
