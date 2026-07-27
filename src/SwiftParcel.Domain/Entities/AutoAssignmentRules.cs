namespace SwiftParcel.Domain.Entities;

public class AutoAssignmentRules
{
    public int Id { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string Conditions { get; set; } = string.Empty;
    public int AssignToHandler { get; set; }
    public string AssignToDepartment { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}