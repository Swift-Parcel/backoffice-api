namespace SwiftParcel.Domain.Entities;

public class Region
{
    public int Id { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public TimeOnly BusinessHoursStart  { get; set; }
    public TimeOnly BusinessHoursEnd { get; set; }
    public DayOfWeek[] BusinessDays = new DayOfWeek[7];
    public string ManagerEmail { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}