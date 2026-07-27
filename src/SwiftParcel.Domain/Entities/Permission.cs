namespace SwiftParcel.Domain.Entities;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Granular { get; set; }
}