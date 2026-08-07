namespace SwiftParcel.Domain.Entities;

using Enums;

public class Parcel
{
    public int Id { get; set; }
    public TrackingNumber TrackingNumber { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    
    public Address SenderAddress { get; set; }

    public string RecipientName { get; set; } = string.Empty;
    public Address RecipientAddress { get; set; }

    public float Weight { get; set; }

    public int Width { get; set; }
    public int Length { get; set; }
    public int Height { get; set; }
    public ParcelStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public ServiceType ServiceType { get; set; }
    public float DeclaredValueInEuros { get; set; }
    public DateTime? PreferredPickupDate { get; set; }
    public Timeslot? PreferredPickupTimeslot { get; set; }

    public ICollection<Case> Cases { get; set; } = new List<Case>();
}