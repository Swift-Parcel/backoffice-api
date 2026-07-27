using System.Text.Json;

namespace SwiftParcel.Domain.Entities;

public class SystemConfig
{
    public int Id { get; set; }
    public string ConfigKey { get; set; } = string.Empty;
    public JsonDocument? ConfigValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public int UpdatedBy { get; set; }
    public DateTime UpdatedDate { get; set; }
}