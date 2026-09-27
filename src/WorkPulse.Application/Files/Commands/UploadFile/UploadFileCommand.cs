using System.IO;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Files.Dtos;

namespace WorkPulse.Application.Files.Commands.UploadFile;

public sealed record UploadFileCommand(Stream Content, string FileName, string ContentType, long SizeBytes, FileEntityType EntityType, Guid EntityId) : IRequest<Result<StoredFileDto>>, IBaseRequest, ICommand;
