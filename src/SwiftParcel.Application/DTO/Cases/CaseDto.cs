using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Cases;

public class CaseDto
{
    public string CaseNumber { get; set; } = string.Empty;
    public CaseType CaseType { get; set; }
    public CaseStatus Status { get; set; }
    public int RegionId { get; set; }
    public int? HandlerId { get; set; }
    public bool IsEscalated { get; set; }
}