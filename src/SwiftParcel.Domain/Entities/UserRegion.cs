namespace SwiftParcel.Domain.Entities;

public class UserRegion
{
    public int UserId { get; set; }
    public int RegionId { get; set; }
    
    public User? User { get; set; }
    public Region? Region { get; set; }
}