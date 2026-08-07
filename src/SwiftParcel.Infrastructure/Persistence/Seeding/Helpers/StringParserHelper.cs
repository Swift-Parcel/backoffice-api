using System.Globalization;
using System.Text.RegularExpressions;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;

public static partial class StringParserHelper
{
    private static readonly Regex DigitsRegex = new(@"\d+", RegexOptions.Compiled);
    private static readonly Regex IntegerRegex = new Regex(@"-?\d+", RegexOptions.Compiled);
    private static readonly Regex DecimalRegex = new(@"\d+(\.\d+)?", RegexOptions.Compiled);

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
            int.TryParse(matches[0].Value, out var w) &&
            int.TryParse(matches[1].Value, out var l) &&
            int.TryParse(matches[2].Value, out var h))
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
    
    /// <summary>
    /// Extracts an integer number from formatted strings ("15 kg", "€50", "-10 pcs")
    /// Returns 0 by as a fallback value.
    /// </summary>
    public static int ExtractInteger(string? input)
    {
        var extracted = ExtractIntegerOrNull(input);
        return extracted ?? 0;
    }
    
    /// <summary>
    /// Extracts an integer number from formatted strings ("15 kg", "€50", "-10 pcs")
    /// Returns null on unsuccessful extraction.
    /// </summary>
    public static int? ExtractIntegerOrNull(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        var match = IntegerRegex.Match(input);
        return match.Success && int.TryParse(match.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var val)
            ? val
            : null;
    }

    /// <summary>
    /// Splits character-separated strings with a chosen delimiter.
    /// </summary>
    public static IEnumerable<string> ParseSeparatedString(string? input, char delimiter)
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

    public static bool IsBooleanString(string input)
    {
        var val = input.Trim().ToLower();
        return val is "yes" or "no" or "true" or "false" or "1" or "0" or "y" or "n" or "t" or "f" or "internal";
    }

    public static float? ParseWeight(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput)) return null;

        string cleaned = rawInput.ToLowerInvariant().Replace(" ", "");
        var match = Regex.Match(cleaned, @"^(\d+(?:\.\d+)?)(?:kg|g|gramm|gram|kilo|kilogramm|t|tonna|ton|lb|lbs|font)?$");

        if (!match.Success || !float.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            return null;

        return match.Groups[2].Value switch
        {
            "g" or "gramm" or "gram" => value / 1000,
            "t" or "tonna" or "ton" => value * 1000,
            "lb" or "lbs" or "font" => value * 0.45359237f,
            _ => value // Default to kg
        };
    }
    
    /// <summary>
    /// Normalizes email address by removing spaces, converting to lower case,
    /// and filtering out missing or invalid values.
    /// </summary>
    public static string? NormalizeEmailOrDefault(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var cleaned = email.Trim().ToLowerInvariant();

        // Filter out placeholder text from legacy database
        if (cleaned is "not provided" or "n/a" or "na" or "none" or "null" or "-")
            return null;

        // Ensure it contains basic email format (something@something.something)
        if (!cleaned.Contains('@') || !cleaned.Contains('.'))
            return null;

        return cleaned;
    }
}