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
    public int HandlerId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime ResolvedDate { get; set; }
    public DateTime SlaDeadline { get; set; }
    public int Region { get; set; }
    public Channel Channel { get; set; }
    public int EscalatedTo { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public int SatisfactionScore { get; set; }
}