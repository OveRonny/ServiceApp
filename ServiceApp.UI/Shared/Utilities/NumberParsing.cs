using System.Globalization;

namespace ServiceApp.UI.Shared.Utilities;

public static class NumberParsing
{
    /// <summary>
    /// Attempts to parse a localized decimal. Returns true on success.
    /// On success: value set, errorMessage = null.
    /// On failure: value = 0 (unchanged conceptually), errorMessage contains localized error.
    /// Empty/whitespace input counts as success and sets value = 0.
    /// </summary>
    public static bool TryParseDecimal(
        string? input,
        out decimal value,
        out string? errorMessage,
        bool forbidDot = true,
        CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;

        if (string.IsNullOrWhiteSpace(input))
        {
            value = 0m;
            errorMessage = null;
            return true;
        }

        if (forbidDot && input.Contains('.'))
        {
            value = 0m;
            errorMessage = "Bruk komma (,) som desimaltegn, ikke punktum (.)";
            return false;
        }

        if (decimal.TryParse(input, NumberStyles.Number, culture, out var parsed))
        {
            value = parsed;
            errorMessage = null;
            return true;
        }

        value = 0m;
        errorMessage = "Ugyldig tallformat. Bruk komma (,) som desimaltegn.";
        return false;
    }
}