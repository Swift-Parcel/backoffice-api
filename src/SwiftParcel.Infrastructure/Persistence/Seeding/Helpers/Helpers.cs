using System.Text.RegularExpressions;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;

public class StringParserHelpers
{
    public static float? ParseWeight(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            Console.WriteLine("Some parcel doesnt have a weight value, returned null.");
            return null;
        }

        string cleaned = rawInput.ToLowerInvariant().Replace(" ", "");

        var match = Regex.Match(cleaned, @"^(\d+(?:\.\d+)?)(?:kg)?");

        if (!match.Success)
        {
            Console.WriteLine("Some parcel doesnt have a weight value with correct format, " +
                              "returned null.");
            return null;
        }

        var value = float.Parse(match.Groups[1].Value);

        var unit = match.Groups[2].Value;

        return unit switch
        {
            "g" or "gramm" or "gram" => value / 1000,
            "kg" or "kilo" or "kilogramm" or "" => value,
            "t" or "tonna" or "ton" => value * 1000,
            "lb" or "lbs" or "font" => value * 0.45359237f,
            _ => null
        };
    }

    public static int? ParseEuro(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            Console.WriteLine("Some parcel doesnt have a declare value, returned null.");
            return null;
        }

        string cleaned = rawInput.ToLowerInvariant().Replace(" ", "");

        var match = Regex.Match(cleaned, @"(\d+)");

        if (!match.Success)
        {
            Console.WriteLine("Some parcel doesnt have a declared value with correct format, " +
                              "returned null.");
            return null;
        }

        var value = int.Parse(match.Groups[1].Value);

        return value;
    }

    public static (int Width, int Length, int Height)? ParseDimensionalValues(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            Console.WriteLine("Some parcel doesnt have a size, returned null.");
            return null;
        }

        string cleaned = rawInput.ToLowerInvariant().Replace(" ", "");

        var match = Regex.Match(cleaned, @"^(\d+)x(\d+)x(\d)");

        if (!match.Success)
        {
            Console.WriteLine("Some parcel doesnt have a size with correct format, returned null.");
            return null;
        }

        int width = int.Parse(match.Groups[1].Value);
        int length = int.Parse(match.Groups[2].Value);
        int height = int.Parse(match.Groups[3].Value);

        return (width, length, height);
    }

    public string? ParseTimeZone(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            Console.WriteLine("Region doesnt have a timezone, returned null.");
            return null;
        }

        return "Europe/Budapest";
    }

    public IEnumerable<DayOfWeek>? ConvertToDaysOfWeek(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            Console.WriteLine("Region doesnt have a timezone, returned null.");
            return null;
        }

        return new List<DayOfWeek>()
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };
    }
    
    private static HashSet<string> LoadAllEnumNames()
    {
        var domainAssembly = typeof(SwiftParcel.Domain.Entities.Tag).Assembly; 
        
        return domainAssembly.GetTypes()
            .Where(t => t.IsEnum && t.Namespace != null && t.Namespace.StartsWith("SwiftParcel.Domain.Enums"))
            .SelectMany(Enum.GetNames)
            .ToHashSet();
    }
}