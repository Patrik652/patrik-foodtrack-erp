namespace FoodTrack.Mobile;

/// <summary>
/// Parses query-string attributes passed through MAUI Shell navigation.
/// </summary>
public static class NavigationQueryHelper
{
    public static bool TryGetGuid(IDictionary<string, object> query, string key, out Guid value)
    {
        value = Guid.Empty;
        if (!query.TryGetValue(key, out var rawValue))
        {
            return false;
        }

        var text = rawValue?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        return Guid.TryParse(Uri.UnescapeDataString(text), out value);
    }

    public static string? GetString(IDictionary<string, object> query, string key)
    {
        if (!query.TryGetValue(key, out var rawValue))
        {
            return null;
        }

        var text = rawValue?.ToString();
        return string.IsNullOrWhiteSpace(text)
            ? null
            : Uri.UnescapeDataString(text);
    }
}
