using System.Text;

namespace WorkPulse.API.Configuration;

/// <summary>
/// Fails startup when the signing key is missing, too short, or still the committed placeholder.
/// A weak or well-known key means every access token in the system is forgeable, and without
/// this guard a misconfigured deploy starts up and accepts them silently.
/// </summary>
public static class JwtSettingsValidation
{
	/// <summary>HS256 keys shorter than the 256-bit hash output weaken the signature.</summary>
	private const int MinimumKeyBytes = 32;

	private const string CommittedPlaceholder =
		"CHANGE_ME_USE_A_STRONG_SECRET_KEY_IN_PRODUCTION_MIN_32_CHARS";

	public static string ValidateAndGetSecretKey(IConfiguration configuration, IHostEnvironment environment)
	{
		IConfigurationSection section = configuration.GetSection("JwtSettings");
		string? secretKey = section["SecretKey"];

		if (string.IsNullOrWhiteSpace(secretKey))
		{
			throw new InvalidOperationException(
				"JwtSettings:SecretKey is not configured. Set it via user-secrets, an environment " +
				"variable, or your secret store before starting the API.");
		}

		if (Encoding.UTF8.GetByteCount(secretKey) < MinimumKeyBytes)
		{
			throw new InvalidOperationException(
				$"JwtSettings:SecretKey must be at least {MinimumKeyBytes} bytes for HS256 signing.");
		}

		// The placeholder is fine for a local dev run but must never reach a deployed environment.
		if (secretKey.Contains(CommittedPlaceholder, StringComparison.Ordinal) && !environment.IsDevelopment())
		{
			throw new InvalidOperationException(
				"JwtSettings:SecretKey is still the placeholder value from appsettings.json. " +
				$"Provide a real secret for the '{environment.EnvironmentName}' environment.");
		}

		if (string.IsNullOrWhiteSpace(section["Issuer"]) || string.IsNullOrWhiteSpace(section["Audience"]))
		{
			throw new InvalidOperationException("JwtSettings:Issuer and JwtSettings:Audience are required.");
		}

		return secretKey;
	}
}
