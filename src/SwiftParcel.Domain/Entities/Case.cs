namespace SwiftParcel.Domain.Entities;

using Enums;

public class Case
{
    public int Id { get; set; }
    public string CaseNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CaseType CaseType { get; set; }
    public CaseStatus Status { get; set; }
    public Priority Priority { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public int? HandlerId { get; set; }
    public Handler? Handler { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsEscalated { get; set; } = false;
    public DateTime? ResolvedDate { get; set; }
    public DateTime SlaDeadline { get; set; }
    public int RegionId { get; set; }
    public Region Region { get; set; } = null!;
    public Channel Channel { get; set; }
    public string? Resolution { get; set; } = string.Empty;
    public int? SatisfactionScore { get; set; }
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<CaseNote> Notes { get; set; } = new List<CaseNote>();
    public ICollection<Parcel> Parcels { get; set; } = new List<Parcel>();
    
    /// <summary>
    /// Centralized definition of active statuses
    /// </summary>
    public static readonly CaseStatus[] ActiveStatuses = 
    {
        CaseStatus.Open,
        CaseStatus.InProgress,
        CaseStatus.Escalated,
        CaseStatus.AwaitingCustomer
    };
    
    /// <summary>
    /// Factory method for creating Delivery Change cases. 
    /// </summary>
    public static Case CreateForDeliveryChange(
        string caseNumber,
        Customer customer,
        Parcel parcel,
        int regionId,
        DateTime? newDate,
        Timeslot? newTimeslot,
        int slaHours)
    {
        var now = DateTime.UtcNow;
        var isVip = customer.Vip;

        var description = $"Delivery change requested.\nNew date: {newDate:yyyy-MM-dd HH:mm UTC} | Timeslot: {newTimeslot.ToString()}";

        return new Case
        {
            CaseNumber = caseNumber,
            Title = $"Delivery Change - Tracking: {parcel.TrackingNumber}",
            Description = description,
            CaseType = CaseType.DeliveryChange,
            Status = CaseStatus.Open,
            Priority = isVip ? Priority.High : Priority.Low,
            Customer = customer,
            RegionId = regionId,
            Channel = Channel.Portal,
            CreatedDate = now,
            SlaDeadline = now.AddHours(slaHours),
            Parcels = new List<Parcel> { parcel },
            Tags = new List<Tag>()
        };
    }
}