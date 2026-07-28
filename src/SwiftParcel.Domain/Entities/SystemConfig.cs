using System.Text.Json;

namespace SwiftParcel.Domain.Entities;

public class SystemConfig
{
    public int Id { get; set; }
    public string ConfigKey { get; set; } = string.Empty;
    public JsonDocument? ConfigValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public int UpdatedById { get; set; }
    public User UpdatedBy { get; set; } = null!;
    public DateTime UpdatedDate { get; set; }
}