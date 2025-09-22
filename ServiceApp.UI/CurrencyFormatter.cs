using System.Globalization;

namespace ServiceApp.UI;

public class CurrencyFormatter
{
    private static readonly CultureInfo NorwegianCulture = CultureInfo.GetCultureInfo("nb-NO");

    public static string FormatKr(decimal? amount)
    {
        if (!amount.HasValue) return string.Empty;
        return string.Format(NorwegianCulture, "{0:N2} kr", amount.Value);
    }
}
