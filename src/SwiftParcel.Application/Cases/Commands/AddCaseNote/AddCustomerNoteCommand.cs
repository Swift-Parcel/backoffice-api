using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Cases.Commands.AddCaseNote;

public record AddCustomerNoteCommand(
    string CaseNumber,
    string Message,
    string CustomerEmail,
    string? Attachment
) : IRequest<Result<int>>;