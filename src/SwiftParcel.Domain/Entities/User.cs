namespace SwiftParcel.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public ICollection<Region> Regions { get; set; }= new List<Region>();
    public string Email { get; set; } = string.Empty;
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
}