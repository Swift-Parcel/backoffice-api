using System.Text.Json;
using System.Text.RegularExpressions;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;

public class StringParserHelper
{
    /**
     * Strips "AZ123", "C45", "U99", etc -> returns 123, 45, 99
     */
    public static int ExtractIntegerId(string legacyId)
    {
        if (string.IsNullOrWhiteSpace(legacyId)) return 0;
        var match = Regex.Match(legacyId, @"\d+");
        return match.Success ? int.Parse(match.Value) : 0;
    }

    /**
     * Converts "yes"/"no", "1"/"0", "true"/"false", etc -> bool
     */
    public static bool ParseBoolean(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var clean = input.Trim().ToLower();
        return clean is "yes" or "1" or "true" or "y" or "t" or "internal";
    }
    
    /**
     * Splits "10x20x30 cm" -> (10, 20, 30)
     */
    public static (int width, int length, int height) ParseDimensions(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return (0, 0, 0);
        var matches = Regex.Matches(raw, @"\d+");
        if (matches.Count >= 3)
        {
            return (int.Parse(matches[0].Value), int.Parse(matches[1].Value), int.Parse(matches[2].Value));
        }

        return (0, 0, 0);
    }

    /**
     * Extracts number from "15 kg" or "€50" -> 15.0 or 50.0
     */
    public static decimal ExtractDecimal(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return 0;
        var match = Regex.Match(input, @"\d+(\.\d+)?");
        return match.Success ? decimal.Parse(match.Value) : 0;
    }
    
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };

    /**
     * Parses a JSON string into a strongly-typed object.
     */
    public static T? ParseJson<T>(string? rawJson, T? fallback = default)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return fallback;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(rawJson.Trim(), DefaultJsonOptions) ?? fallback;
        }
        catch (JsonException)
        {
            return fallback;
        }
    }

   
    /**
     * Parses a JSON string into a JsonDocument for dynamic/unstructured extraction when 
     * a fixed target model does not exist yet.
     */
    public static JsonDocument? ParseJsonDocument(string? rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return null;
        }

        try
        {
            return JsonDocument.Parse(rawJson.Trim());
        }
        catch (JsonException)
        {
            return null;
        }
    }
}