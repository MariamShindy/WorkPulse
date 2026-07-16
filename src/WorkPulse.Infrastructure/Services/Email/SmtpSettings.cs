namespace WorkPulse.Infrastructure.Services.Email;

public sealed class SmtpSettings
{
	public const string SectionName = "SmtpSettings";

	public string Host { get; set; } = "localhost";

	public int Port { get; set; } = 587;

	public string? Username { get; set; }

	public string? Password { get; set; }

	public bool EnableSsl { get; set; } = true;

	public string FromAddress { get; set; } = "noreply@workpulse.local";

	public string FromName { get; set; } = "WorkPulse";
}
