namespace SwiftParcel.Application.Helpers;

public class FormatHelper
{
    public static string FormatTrackingNumber(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
        {
            return trackingNumber;
        }

        var normalized = trackingNumber.Trim().ToUpperInvariant();

        if (normalized.StartsWith("SP") && !normalized.StartsWith("SP-"))
        {
            return normalized.Insert(2, "-");
        }

        return normalized;
    }
}