namespace SwiftParcel.Domain.Entities;

public class CaseNote
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public Case Case { get; set; } = null!;
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public string NoteText { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsInternal { get; set; }
    public string Attachment { get; set; } = string.Empty;
}