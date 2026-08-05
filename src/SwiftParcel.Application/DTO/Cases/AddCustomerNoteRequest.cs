namespace SwiftParcel.Application.DTO.Cases;

public record AddCustomerNoteRequest(
    string Message, 
    string CustomerEmail, 
    string? Attachment = null
);