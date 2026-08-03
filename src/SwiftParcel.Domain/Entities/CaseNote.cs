namespace SwiftParcel.Domain.Entities;

public class CaseNote
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public string NoteText { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsInternal { get; set; }
    public int? HandlerId { get; set; }
    public int? CustomerId { get; set; }
    public string Attachment { get; set; } = string.Empty;
    
    public Case Case { get; set; } = null!;
    public User? Handler { get; set; }
    public Customer? Customer { get; set; }
}