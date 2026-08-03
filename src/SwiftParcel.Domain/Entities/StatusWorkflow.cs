namespace SwiftParcel.Domain.Entities;

using Enums;

public class StatusWorkflow
{
    public int Id { get; set; }
    public CaseStatus? FromStatus { get; set; }
    public CaseStatus? ToStatus { get; set; }
    public bool RequireNote { get; set; }
    public bool RequireResolution { get; set; }
    public ICollection<Role> AllowedRoles { get; set; } = new List<Role>();
}