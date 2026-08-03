namespace SwiftParcel.Domain.Entities;
using Enums;

public class AuditLog
{
    public int Id { get; set; }
    public AuditAction AuditAction { get; set; }
    public EntityType EntityType { get; set; }
    public int? EntityId { get; set; }
    public int UserId  { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime? TimeStamp { get; set; }
    public System.Net.IPAddress? IpAddress { get; set; }
    public string? Details { get; set; }
    
}