using System;
using System.IO;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Files.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Files.Commands.UploadFile;

public sealed record UploadFileCommand(Stream Content, string FileName, string ContentType, long SizeBytes, FileEntityType EntityType, Guid EntityId) : IRequest<Result<StoredFileDto>>, IBaseRequest, ICommand;
