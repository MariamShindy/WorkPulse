using System.Text.Json;

namespace WorkPulse.Infrastructure.Services.Ai;

internal static class OllamaToolArgumentParser
{
	internal const int DefaultMonths = 6;
	internal const int MinMonths = 1;
	internal const int MaxMonths = 24;

	internal static int ReadMonths(JsonElement? arguments)
	{
		if (arguments is not { ValueKind: JsonValueKind.Object } argumentObject)
		{
			return DefaultMonths;
		}

		if (!argumentObject.TryGetProperty("months", out JsonElement monthsElement))
		{
			return DefaultMonths;
		}

		int months = monthsElement.ValueKind switch
		{
			JsonValueKind.Number when monthsElement.TryGetInt32(out int parsedMonths) => parsedMonths,
			JsonValueKind.String when int.TryParse(monthsElement.GetString(), out int parsedMonths) => parsedMonths,
			_ => DefaultMonths
		};

		return Math.Clamp(months, MinMonths, MaxMonths);
	}
}
