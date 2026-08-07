namespace SwiftParcel.Application.DTO.Cases;

public record CaseNoteDto(
     DateTime Timestamp,
     string Note,
     
     int? HandlerId,
     string? HandlerName,
     
     int? CustomerId,
     string? CustomerName,
     
     string? Attachment
);