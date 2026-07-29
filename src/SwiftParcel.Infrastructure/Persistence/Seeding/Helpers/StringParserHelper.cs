using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Mail;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;

public partial static class StringParserHelper
{
    private static readonly Regex DigitsRegex = new(@"\d+", RegexOptions.Compiled);
    private static readonly Regex DecimalRegex = new(@"\d+(\.\d+)?", RegexOptions.Compiled);

    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };

    /// <summary>
    /// Strips non-digits ("AZ123", "C45") and returns the parsed integer.
    /// </summary>
    public static int ExtractIntegerId(string? legacyId)
    {
        if (string.IsNullOrWhiteSpace(legacyId)) return 0;
        
        var match = DigitsRegex.Match(legacyId);
        return match.Success && int.TryParse(match.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) 
            ? id 
            : 0;
    }

    /// <summary>
    /// Converts boolean representations ("yes"/"no", "1"/"0", "true"/"false") to a bool.
    /// </summary>
    public static bool ParseBoolean(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        
        var clean = input.Trim().ToLower();
        return clean is "yes" or "1" or "true" or "y" or "t" or "internal";
    }
    
    /// <summary>
    /// Splits "10x20x30 cm" into a (width, length, height) tuple.
    /// </summary>
    public static (int width, int length, int height) ParseDimensions(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return (0, 0, 0);

        var matches = DigitsRegex.Matches(raw);
        if (matches.Count >= 3 &&
            int.TryParse(matches[0].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var w) &&
            int.TryParse(matches[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var l) &&
            int.TryParse(matches[2].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var h))
        {
            return (w, l, h);
        }

        return (0, 0, 0);
    }

    /// <summary>
    /// Extracts a decimal number from formatted strings ("15 kg", "€50").
    /// </summary>
    public static decimal ExtractDecimal(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return 0m;

        var match = DecimalRegex.Match(input);
        return match.Success && decimal.TryParse(match.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var val)
            ? val
            : 0m;
    }
    
    private static readonly Regex IntegerRegex = new Regex(@"-?\d+", RegexOptions.Compiled);

    /// <summary>
    /// Extracts an integer number from formatted strings ("15 kg", "€50", "-10 pcs").
    /// </summary>
    public static int ExtractInteger(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return 0;

        var match = IntegerRegex.Match(input);
        return match.Success && int.TryParse(match.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var val)
            ? val
            : 0;
    }

    /// <summary>
    /// Splits character-separated strings with a chosen delimiter.
    /// </summary>
    private static IEnumerable<string> ParseSeparatedString(string? input, char delimiter)
    {
        if (string.IsNullOrWhiteSpace(input)) return [];

        return input
            .Split(delimiter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct();
    }
    
    /// <summary>
    /// Splits comma-separated strings ("SP-101, SP-102") into an array of strings.
    /// </summary>
    public static IEnumerable<string> ParseCsvString(string? input)
    {
        return ParseSeparatedString(input, ',');
    }
    
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
        if (IsBooleanString(clean))
        {
            return JsonDocument.Parse(ParseBoolean(clean) ? "true" : "false");
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
            var items = ParseSeparatedString(clean, delimiter);
            return JsonSerializer.SerializeToDocument(items);
        }

        // Fallback string primitive
        return JsonDocument.Parse(JsonSerializer.Serialize(clean));
    }
    
    private static bool IsBooleanString(string input)
    {
        var val = input.Trim().ToLower();
        return val is "yes" or "no" or "true" or "false" or "1" or "0" or "y" or "n" or "t" or "f" or "internal";
    }
    
    [GeneratedRegex(@"^(?:\+|00)?\d{7,15}$", RegexOptions.Compiled)]
    private static partial Regex PhoneRegex();

    /// <summary>
    /// Normalizes phone numbers into E.164 format.
    /// Converts leading '00' to '+', strips spaces/dashes, and verifies digit count.
    /// </summary>
    public static bool TryNormalizePhoneNumber(string? input, [NotNullWhen(true)] out string? normalized)
    {
        normalized = null;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        ReadOnlySpan<char> cleanedSpan = input
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("(", "")
            .Replace(")", "");

        string cleaned = cleanedSpan.ToString();

        if (cleaned.StartsWith("00", StringComparison.Ordinal))
        {
            cleaned = "+" + cleaned[2..];
        }

        if (!PhoneRegex().IsMatch(cleaned))
            return false;

        normalized = cleaned.StartsWith('+') ? cleaned : "+" + cleaned;
        return true;
    }

    /// <summary>
    /// Normalizes email addresses by trimming whitespace, lowercasing, and validating standard syntax.
    /// </summary>
    public static bool TryNormalizeEmail(string? input, [NotNullWhen(true)] out string? normalized)
    {
        normalized = null;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        string trimmed = input.Trim().ToLowerInvariant();

        if (!MailAddress.TryCreate(trimmed, out var address) || address.Address != trimmed)
            return false;

        int lastDotIndex = trimmed.LastIndexOf('.');
        int atIndex = trimmed.IndexOf('@');

        if (atIndex <= 0 || lastDotIndex <= atIndex + 1 || lastDotIndex == trimmed.Length - 1)
            return false;

        normalized = trimmed;
        return true;
    }
}