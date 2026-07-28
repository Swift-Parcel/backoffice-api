using System.Globalization;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;

public class TimestampParserHelper
{
    private static readonly string[] GlobalFormats = new string[]
    {
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd",
        "dd/MM/yyyy HH:mm:ss",
        "dd/MM/yyyy",
        "MM/dd/yyyy HH:mm:ss",
        "MM/dd/yyyy",
        "yyyy/MM/dd HH:mm:ss",
        "yyyy/MM/dd",
    };

    public static bool TryParse(string input, out DateTime result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(input) || input == "[null]")
        {
            return false;
        }
        
        return DateTime.TryParseExact(
            input?.Trim(), 
            GlobalFormats, 
            CultureInfo.InvariantCulture, 
            DateTimeStyles.None, 
            out result
        );
    }
}