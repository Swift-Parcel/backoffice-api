using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Domain.ValueObjects;

public readonly record struct TrackingNumber
{
    public static readonly Error Empty = Error.Validation("Tracking number cannot be empty or whitespace.");

    public string Value { get; }

    private TrackingNumber(string value)
    {
        Value = value;
    }

    public static Result<TrackingNumber> Create(string rawTrackingNumber)
    {
        if (string.IsNullOrWhiteSpace(rawTrackingNumber))
        {
            return Result<TrackingNumber>.Failure(Empty);
        }

        var normalized = rawTrackingNumber.Trim().ToUpperInvariant();

        if (normalized.StartsWith("SP") && !normalized.StartsWith("SP-"))
        {
            normalized = normalized.Insert(2, "-");
        }

        return Result<TrackingNumber>.Success(new TrackingNumber(normalized));
    }

    public static implicit operator string(TrackingNumber trackingNumber) => trackingNumber.Value;
    public override string ToString() => Value;
}