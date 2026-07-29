namespace SwiftParcel.Domain.Entities;

public class UserPermission
{
    public int UserId { get; set; }
    public int PermissionId { get; set; }
    public DateTime? Expires { get; set; }
    
    public User? User { get; set; }
    public Permission? Permission { get; set; }
}