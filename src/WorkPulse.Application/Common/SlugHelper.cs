using System.Text.RegularExpressions;

namespace WorkPulse.Application.Common;

public static partial class SlugHelper
{
    public static string ToSlug(string value)
    {
        var slug = value.Trim().ToLowerInvariant();
        slug = NonAlphaNumeric().Replace(slug, "-");
        slug = MultipleHyphens().Replace(slug, "-").Trim('-');
        return slug.Length > 0 ? slug : "company";
    }

    public static string ToKey(string value)
    {
        var key = NonAlphaNumericKey().Replace(value.ToUpperInvariant(), "");
        return key.Length > 0 ? key[..Math.Min(key.Length, 6)] : "TEAM";
    }

    [GeneratedRegex("[^a-z0-9\\s-]")]
    private static partial Regex NonAlphaNumeric();

    [GeneratedRegex("-{2,}")]
    private static partial Regex MultipleHyphens();

    [GeneratedRegex("[^A-Z0-9]")]
    private static partial Regex NonAlphaNumericKey();
}
