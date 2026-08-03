namespace SwiftParcel.Application.DTO.Cases;

public record AddHandlerNoteRequest(
    string Message, 
    bool IsInternal, 
    string? Attachment
);