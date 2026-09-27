using System.Text.Json;
using WorkPulse.Infrastructure.Services.Ai;

namespace WorkPulse.Infrastructure.Tests.Services;

public sealed class OllamaToolArgumentParserTests
{
	[Fact]
	public void ReadMonths_ReturnsDefault_WhenArgumentsMissing()
	{
		Assert.Equal(OllamaToolArgumentParser.DefaultMonths, OllamaToolArgumentParser.ReadMonths(null));
	}

	[Theory]
	[InlineData(0, 1)]
	[InlineData(-5, 1)]
	[InlineData(1, 1)]
	[InlineData(24, 24)]
	[InlineData(100, 24)]
	public void ReadMonths_ClampsToSupportedRange(int input, int expected)
	{
		using JsonDocument document = JsonDocument.Parse($"{{\"months\": {input}}}");
		Assert.Equal(expected, OllamaToolArgumentParser.ReadMonths(document.RootElement));
	}

	[Fact]
	public void ReadMonths_ParsesStringValues()
	{
		using JsonDocument document = JsonDocument.Parse("{\"months\": \"12\"}");
		Assert.Equal(12, OllamaToolArgumentParser.ReadMonths(document.RootElement));
	}

	[Fact]
	public void ReadMonths_FallsBackToDefault_WhenValueIsInvalid()
	{
		using JsonDocument document = JsonDocument.Parse("{\"months\": \"not-a-number\"}");
		Assert.Equal(OllamaToolArgumentParser.DefaultMonths, OllamaToolArgumentParser.ReadMonths(document.RootElement));
	}
}
