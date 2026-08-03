using System.Reflection;
using System.Text.RegularExpressions;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;

public class EnumParserHelper
{
    /// <summary>
    /// Returns raw enum member names converted directly to lowercase.
    /// </summary>
    public static HashSet<string> GetEnumNamesLowercase<TMarker>()
    {
        return GetEnumNamesFromAssembly(typeof(TMarker).Assembly)
            .Select(name => name.ToLowerInvariant())
            .ToHashSet();
    }

    /// <summary>
    /// Returns enum member names converted to lowercase snake_case.
    /// </summary>
    public static HashSet<string> GetEnumNamesSnakeCase<TMarker>()
    {
        return GetEnumNamesFromAssembly(typeof(TMarker).Assembly)
            .Select(name => Regex.Replace(name, "(?<!^)([A-Z])", "_$1").ToLowerInvariant())
            .ToHashSet();
    }

    private static HashSet<string> GetEnumNamesFromAssembly(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => t.IsEnum && t.Namespace != null && t.Namespace.StartsWith("SwiftParcel.Domain.Enums"))
            .SelectMany(Enum.GetNames)
            .ToHashSet();
    }
}