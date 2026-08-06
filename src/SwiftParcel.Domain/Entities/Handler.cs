namespace SwiftParcel.Domain.Entities;

public class Handler
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Department { get; set; } = string.Empty; //could be an enum
    public DateTime HireDate { get; set; }
    public int MaxCases { get; set; }
    
    public ICollection<Case> Cases { get; set; } = new List<Case>();
    public bool IsActive { get; set; } = true;
}