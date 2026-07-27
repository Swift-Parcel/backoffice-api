namespace SwiftParcel.Domain.Entities;

public class HolidayRegions
{
    public int HolidayId { get; set; }
    public int RegionId { get; set; }
    
    public Holiday? Holiday { get; set; }
    public Region?  Region { get; set; }
}