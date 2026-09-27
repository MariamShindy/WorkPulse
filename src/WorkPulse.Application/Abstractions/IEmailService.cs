namespace WorkPulse.Application.Abstractions;

public interface IEmailService
{
	Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default(CancellationToken));

	Task SendTemplatedAsync(string to, string templateName, object model, CancellationToken ct = default(CancellationToken));
}
