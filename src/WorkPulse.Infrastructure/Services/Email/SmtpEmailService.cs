using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorkPulse.Application.Abstractions;

namespace WorkPulse.Infrastructure.Services.Email;

public sealed class SmtpEmailService(IOptions<SmtpSettings> options, ILogger<SmtpEmailService> logger) : IEmailService
{
	public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default(CancellationToken))
	{
		SmtpSettings settings = options.Value;
		using SmtpClient client = new SmtpClient(settings.Host, settings.Port)
		{
			EnableSsl = settings.EnableSsl,
			Credentials = (string.IsNullOrWhiteSpace(settings.Username) ? null : new NetworkCredential(settings.Username, settings.Password))
		};
		using MailMessage message = new MailMessage
		{
			From = new MailAddress(settings.FromAddress, settings.FromName),
			Subject = subject,
			Body = htmlBody,
			IsBodyHtml = true
		};
		message.To.Add(to);
		await client.SendMailAsync(message, ct);
		logger.LogInformation("Email sent to {Recipient} with subject {Subject}", to, subject);
	}

	public Task SendTemplatedAsync(string to, string templateName, object model, CancellationToken ct = default(CancellationToken))
	{
		string htmlBody = $"<p>Template: {templateName}</p><pre>{JsonSerializer.Serialize(model)}</pre>";
		return SendAsync(to, templateName, htmlBody, ct);
	}
}
