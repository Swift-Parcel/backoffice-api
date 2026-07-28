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
    public DateTime UpdatedDate { get; set; }
    public DateTime ResolvedDate { get; set; }
    public DateTime SlaDeadline { get; set; }
    public int RegionId { get; set; }
    public Region Region { get; set; } = null!;
    public Channel Channel { get; set; }
    public int EscalatedToId { get; set; }
    public Handler? EscalatedTo { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public int SatisfactionScore { get; set; }
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<CaseNote> Notes { get; set; } = new List<CaseNote>();
    public ICollection<Parcel> Parcels { get; set; } = new List<Parcel>();
}