using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Files.Commands.DeleteFile;

public sealed record DeleteFileCommand(Guid FileId) : IRequest<Result>, IBaseRequest, ICommand;
