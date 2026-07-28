namespace SwiftParcel.Domain.Entities;

public class Holiday
{
    public int Id { get; set; }
    public string HolidayName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsRecurring { get; set; }
    public string Notes { get; set; } = string.Empty;
    public ICollection<Region> Regions { get; set; } = new List<Region>();
}