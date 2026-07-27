namespace SwiftParcel.Domain.Entities;

public class Handler
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Department { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public int MaxCases { get; set; } = 10;
}