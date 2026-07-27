namespace SwiftParcel.Domain.Entities;

public class CaseNote
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public int AuthorUserId { get; set; }
    public string NoteText { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsInternal { get; set; }
    public string Attachment { get; set; } = string.Empty;
}