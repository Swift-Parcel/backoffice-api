using System.Text.RegularExpressions;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Infrastructure.Parsers;

public static partial class AddressParserHelper
{
    [GeneratedRegex(@"^(?<street>.*?)\s+(?<number>\d+[a-zA-Z\-/]*|\d+\.)\s*$", RegexOptions.Compiled)]
    private static partial Regex StreetAndNumberRegex();

    public static Address SplitStringAddress(string rawAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawAddress);

        var parts = rawAddress
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
        {
            throw new ArgumentException($"Invalid address format: '{rawAddress}'", nameof(rawAddress));
        }

        var city = parts[0];
        var streetAndNumberSegment = parts[1];
        var rawPostalCode = parts.Length >= 3 ? parts[2] : string.Empty;

        var (street, streetNumber) = ParseStreetAndNumber(streetAndNumberSegment);

        var (cleanPostalCode, countryCode) = ParsePostalCodeAndCountry(rawPostalCode, city);

        return new Address
        (
            street,
            streetNumber,
            city,
            cleanPostalCode,
            countryCode
        );
    }

    private static (string Street, string StreetNumber) ParseStreetAndNumber(string rawStreetSegment)
    {
        var match = StreetAndNumberRegex().Match(rawStreetSegment);

        if (match.Success)
        {
            var street = match.Groups["street"].Value.Trim();
            var number = match.Groups["number"].Value.TrimEnd('.').Trim();

            return (street, number);
        }

        return (rawStreetSegment, string.Empty);
    }

    private static (string CleanPostalCode, string CountryCode) ParsePostalCodeAndCountry(string rawPostalCode, string city)
    {
        var countryCode = InferCountryCode(rawPostalCode, city);
        var cleanPostalCode = rawPostalCode;

        if (!string.IsNullOrEmpty(cleanPostalCode) && cleanPostalCode.Contains('-'))
        {
            var parts = cleanPostalCode.Split('-', 2);
            cleanPostalCode = parts[^1].Trim();
        }

        return (cleanPostalCode, countryCode);
    }

    private static string InferCountryCode(string rawPostalCode, string city)
    {
        if (rawPostalCode.StartsWith("H-", StringComparison.OrdinalIgnoreCase)) return "HU";
        if (rawPostalCode.StartsWith("AT-", StringComparison.OrdinalIgnoreCase)) return "AT";
        if (rawPostalCode.StartsWith("CZ-", StringComparison.OrdinalIgnoreCase)) return "CZ";
        if (rawPostalCode.StartsWith("PL-", StringComparison.OrdinalIgnoreCase)) return "PL";

        if (city.StartsWith("Budapest", StringComparison.OrdinalIgnoreCase) || 
            city.Equals("Debrecen", StringComparison.OrdinalIgnoreCase) || 
            city.Equals("Szeged", StringComparison.OrdinalIgnoreCase))
            return "HU";

        if (city.Equals("Wien", StringComparison.OrdinalIgnoreCase) || 
            city.Equals("Graz", StringComparison.OrdinalIgnoreCase) || 
            city.Equals("Linz", StringComparison.OrdinalIgnoreCase) || 
            city.Equals("Salzburg", StringComparison.OrdinalIgnoreCase))
            return "AT";

        if (city.StartsWith("Praha", StringComparison.OrdinalIgnoreCase) || 
            city.Equals("Brno", StringComparison.OrdinalIgnoreCase))
            return "CZ";

        if (city.StartsWith("Warsz", StringComparison.OrdinalIgnoreCase) || 
            city.StartsWith("Warsaw", StringComparison.OrdinalIgnoreCase) || 
            city.Equals("Kraków", StringComparison.OrdinalIgnoreCase) || 
            city.Equals("Gdańsk", StringComparison.OrdinalIgnoreCase))
            return "PL";

        return rawPostalCode switch
        {
            var p when Regex.IsMatch(p, @"^\d{2}-\d{3}$") => "PL",
            var p when Regex.IsMatch(p, @"^\d{3}\s?\d{2}$") => "CZ",
            var p when Regex.IsMatch(p, @"^\d{4}$") => "HU/AT",
            _ => "UNKNOWN"
        };
    }
}