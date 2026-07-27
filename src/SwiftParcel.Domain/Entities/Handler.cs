namespace SwiftParcel.Domain.Entities;

public class Handler
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Department { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public int MaxCases { get; set; }
    
    public ICollection<Case> CasesHandled { get; set; } = new List<Case>();
    public ICollection<Case> CasesEscalatedTo { get; set; } = new List<Case>();
}