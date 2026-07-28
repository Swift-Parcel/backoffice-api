namespace SwiftParcel.Domain.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Case> Cases { get; set; } = new List<Case>();
}