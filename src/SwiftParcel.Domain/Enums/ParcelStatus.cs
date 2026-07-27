namespace SwiftParcel.Domain.Enums;

public enum ParcelStatus
{
    PendingPickup,
    PickedUp,
    InTransit,
    OutForDelivery,
    Delivered,
    DeliveryAttemptFailed,
    Lost,
    Damaged
}