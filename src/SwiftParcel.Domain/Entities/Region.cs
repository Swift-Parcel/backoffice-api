namespace SwiftParcel.Domain.Entities;

public class Region
{
    public int Id { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public Country Country { get; set; } = null!;
    public TimeOnly BusinessHoursStart  { get; set; }
    public TimeOnly BusinessHoursEnd { get; set; }
    public ICollection<DayOfWeek> BusinessDays = new List<DayOfWeek>();
    public string ManagerEmail { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    
    public ICollection<User> Users { get; set; } = new List<User>();
}