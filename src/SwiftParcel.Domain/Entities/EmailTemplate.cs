namespace SwiftParcel.Domain.Entities;

public class EmailTemplate
{
    public int Id { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int Region { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}