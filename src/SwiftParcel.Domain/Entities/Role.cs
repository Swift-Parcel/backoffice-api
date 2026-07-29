namespace SwiftParcel.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool CanAccessAllRegions { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    public ICollection<User> Users { get; set; } = new List<User>();
}