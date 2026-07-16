using System;
using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class StoredFile : TenantEntity
{
	public string FileName { get; set; } = string.Empty;

	public string ContentType { get; set; } = string.Empty;

	public long SizeBytes { get; set; }

	public string StorageKey { get; set; } = string.Empty;

	public Guid UploadedById { get; set; }

	public FileEntityType EntityType { get; set; }

	public Guid EntityId { get; set; }
}
