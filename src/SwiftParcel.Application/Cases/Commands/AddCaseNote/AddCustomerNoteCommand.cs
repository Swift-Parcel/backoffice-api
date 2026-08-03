using MediatR;
using SwiftParcel.Application.Common.Models;

public record AddCustomerNoteCommand(
    string CaseNumber,
    string Message,
    string CustomerEmail,
    string? Attachment
) : IRequest<Result<int>>;