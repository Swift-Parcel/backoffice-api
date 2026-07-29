namespace SwiftParcel.Domain.Entities;

using Enums;

public class SlaRule
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CaseType? CaseType { get; set; }
    public Priority? Priority { get; set; }
    public ServiceType? ServiceType { get; set; }
    public int SlaHours { get; set; }
    public bool IsBusinessHours { get; set; }
    public int EscalationAfter { get; set; }
    public int? EscalationHandlerId { get; set; }
    public Handler? EscalationHandler { get; set; } = null!;
    public string? EscalationDepartment { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}