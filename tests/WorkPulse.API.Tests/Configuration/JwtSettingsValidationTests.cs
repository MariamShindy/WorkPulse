using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using WorkPulse.API.Configuration;

namespace WorkPulse.API.Tests.Configuration;

/// <summary>
/// appsettings.json ships a placeholder signing key. Without a startup guard a deploy that forgot
/// to override it would boot happily and accept forgeable tokens, so these assert it fails fast.
/// </summary>
public sealed class JwtSettingsValidationTests
{
	private const string Placeholder = "CHANGE_ME_USE_A_STRONG_SECRET_KEY_IN_PRODUCTION_MIN_32_CHARS";
	private const string StrongKey = "b8Fq2s9VxL4mR7tZ1pN6yK3wJ5hG0dC8aQeUiOoPlKjHgFdSaZxCvBnM";

	private static IConfiguration Config(string? secret, string? issuer = "WorkPulse", string? audience = "WorkPulse")
	{
		var values = new Dictionary<string, string?>
		{
			["JwtSettings:SecretKey"] = secret,
			["JwtSettings:Issuer"] = issuer,
			["JwtSettings:Audience"] = audience
		};

		return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
	}

	private sealed class Env(string name) : IHostEnvironment
	{
		public string EnvironmentName { get; set; } = name;
		public string ApplicationName { get; set; } = "WorkPulse.API.Tests";
		public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
		public IFileProvider ContentRootFileProvider { get; set; } =
			new Microsoft.Extensions.FileProviders.NullFileProvider();
	}

	private static readonly IHostEnvironment Development = new Env("Development");
	private static readonly IHostEnvironment Production = new Env("Production");

	[Fact]
	public void A_strong_key_is_returned_unchanged()
	{
		Assert.Equal(StrongKey, JwtSettingsValidation.ValidateAndGetSecretKey(Config(StrongKey), Production));
	}

	[Fact]
	public void The_placeholder_is_rejected_in_production()
	{
		var ex = Assert.Throws<InvalidOperationException>(
			() => JwtSettingsValidation.ValidateAndGetSecretKey(Config(Placeholder), Production));

		Assert.Contains("placeholder", ex.Message, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void The_placeholder_is_tolerated_in_development_so_local_runs_still_work()
	{
		Assert.Equal(Placeholder, JwtSettingsValidation.ValidateAndGetSecretKey(Config(Placeholder), Development));
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void A_missing_key_is_rejected(string? secret)
	{
		Assert.Throws<InvalidOperationException>(
			() => JwtSettingsValidation.ValidateAndGetSecretKey(Config(secret), Development));
	}

	[Fact]
	public void A_key_shorter_than_the_hash_output_is_rejected_even_in_development()
	{
		var ex = Assert.Throws<InvalidOperationException>(
			() => JwtSettingsValidation.ValidateAndGetSecretKey(Config("tooshort"), Development));

		Assert.Contains("32 bytes", ex.Message);
	}

	[Fact]
	public void A_key_of_exactly_32_bytes_is_accepted()
	{
		string key = new('k', 32);

		Assert.Equal(key, JwtSettingsValidation.ValidateAndGetSecretKey(Config(key), Production));
	}

	[Fact]
	public void A_31_byte_key_is_rejected_at_the_boundary()
	{
		Assert.Throws<InvalidOperationException>(
			() => JwtSettingsValidation.ValidateAndGetSecretKey(Config(new string('k', 31)), Production));
	}

	[Fact]
	public void Multibyte_characters_count_toward_the_byte_length_not_the_character_length()
	{
		// 20 three-byte characters is 60 bytes, so it passes despite being 20 chars long.
		string multibyte = new('世', 20);

		Assert.Equal(multibyte, JwtSettingsValidation.ValidateAndGetSecretKey(Config(multibyte), Production));

		// 10 of them is only 30 bytes and must fail.
		Assert.Throws<InvalidOperationException>(
			() => JwtSettingsValidation.ValidateAndGetSecretKey(Config(new string('世', 10)), Production));
	}

	[Theory]
	[InlineData(null, "WorkPulse")]
	[InlineData("WorkPulse", null)]
	[InlineData("", "WorkPulse")]
	public void A_missing_issuer_or_audience_is_rejected(string? issuer, string? audience)
	{
		Assert.Throws<InvalidOperationException>(
			() => JwtSettingsValidation.ValidateAndGetSecretKey(Config(StrongKey, issuer, audience), Production));
	}
}
