using FluentValidation;
using Microsoft.Extensions.Options;
using WorkPulse.Application.Abstractions.Options;

namespace WorkPulse.Application.Files.Commands.UploadFile;

public sealed class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
	public UploadFileCommandValidator(IOptions<FileStorageOptions> options)
	{
		RuleFor((UploadFileCommand x) => x.FileName).NotEmpty().MaximumLength(255);
		RuleFor((UploadFileCommand x) => x.ContentType).NotEmpty().MaximumLength(100);
		RuleFor((UploadFileCommand x) => x.EntityId).NotEmpty();
		RuleFor((UploadFileCommand x) => x.SizeBytes).GreaterThan(0L).LessThanOrEqualTo<UploadFileCommand, long>((UploadFileCommand _) => options.Value.MaxFileSizeBytes).WithMessage($"File exceeds maximum size of {options.Value.MaxFileSizeBytes} bytes.");
	}
}
