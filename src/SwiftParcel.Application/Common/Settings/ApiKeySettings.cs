namespace SwiftParcel.Application.Common.Settings;

public class ApiKeySettings
{
    public const string SectionName = "ApiKeySettings";

    public string HeaderName { get; init; } = "X-Api-Key";
    public string SecretKey { get; init; } = string.Empty;
}