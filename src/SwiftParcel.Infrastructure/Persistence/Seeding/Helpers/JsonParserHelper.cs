using System.Globalization;
using System.Text.Json;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;

public class JsonParserHelper
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };

    /// <summary>
    /// Parses a JSON string into a strongly-typed object.
    /// </summary>
    public static T? ParseJson<T>(string? rawJson, T? fallback = default)
    {
        if (string.IsNullOrWhiteSpace(rawJson)) return fallback;

        try
        {
            return JsonSerializer.Deserialize<T>(rawJson.Trim(), DefaultJsonOptions) ?? fallback;
        }
        catch (JsonException)
        {
            return fallback;
        }
    }

    /// <summary>
    /// Parses a JSON string into a JsonDocument for unstructured extraction.
    /// </summary>
    public static JsonDocument? ParseJsonDocument(string? rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson)) return null;

        try
        {
            return JsonDocument.Parse(rawJson.Trim());
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Parses any legacy database string value into a clean JsonDocument (jsonb).
    /// </summary>
    public static JsonDocument ParseToJsonDocument(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return JsonDocument.Parse("null");
        }

        var clean = input.Trim();

        // Existing valid JSON
        try
        {
            return JsonDocument.Parse(clean);
        }
        catch (JsonException)
        {
            // Not native JSON; fallback to heuristics
        }

        // Booleans
        if (StringParserHelper.IsBooleanString(clean))
        {
            return JsonDocument.Parse(StringParserHelper.ParseBoolean(clean) ? "true" : "false");
        }
        
        // Numbers
        if (long.TryParse(clean, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longVal))
        {
            return JsonDocument.Parse(longVal.ToString());
        }

        if (decimal.TryParse(clean, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalVal))
        {
            return JsonDocument.Parse(decimalVal.ToString(CultureInfo.InvariantCulture));
        }

        // Delimited lists
        if (clean.Contains('|') || clean.Contains(','))
        {
            char delimiter = clean.Contains('|') ? '|' : ',';
            var items = StringParserHelper.ParseSeparatedString(clean, delimiter);
            return JsonSerializer.SerializeToDocument(items);
        }

        // Fallback string primitive
        return JsonDocument.Parse(JsonSerializer.Serialize(clean));
    }
}