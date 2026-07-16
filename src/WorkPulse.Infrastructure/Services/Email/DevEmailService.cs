using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WorkPulse.Application.Abstractions;

namespace WorkPulse.Infrastructure.Services.Email;

public sealed class DevEmailService(ILogger<DevEmailService> logger) : IEmailService
{
	public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default(CancellationToken))
	{
		logger.LogInformation("DEV EMAIL -> To: {To}, Subject: {Subject}, Body: {Body}", to, subject, htmlBody);
		return Task.CompletedTask;
	}

	public Task SendTemplatedAsync(string to, string templateName, object model, CancellationToken ct = default(CancellationToken))
	{
		logger.LogInformation("DEV EMAIL (template) -> To: {To}, Template: {Template}, Model: {@Model}", to, templateName, model);
		return Task.CompletedTask;
	}
}
