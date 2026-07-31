using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;

public partial class ContactInfoParserHelper
{
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

        if (PhoneRegex().IsMatch(cleaned))
            return false;

        normalized = cleaned.StartsWith('+') ? cleaned : "+" + cleaned;
        return true;
    }

    /// <summary>
    /// Normalizes a phone number, or returns the specified fallback.
    /// </summary>
    public static string NormalizePhoneNumberOrDefault(string? input, string defaultValue = "")
    {
        return TryNormalizePhoneNumber(input, out var normalized) ? normalized : defaultValue;
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

    /// <summary>
    /// Normalizes an email address, or returns the specified fallback.
    /// </summary>
    public static string NormalizeEmailOrDefault(string? input, string defaultValue = "")
    {
        return TryNormalizeEmail(input, out var normalized) ? normalized : defaultValue;
    }
}