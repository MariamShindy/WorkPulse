using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Files.Commands.DeleteFile;

public sealed record DeleteFileCommand(Guid FileId) : IRequest<Result>, IBaseRequest, ICommand;
