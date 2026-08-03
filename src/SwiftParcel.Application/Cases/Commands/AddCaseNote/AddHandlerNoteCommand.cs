using MediatR;
using SwiftParcel.Application.Common.Models;

public record AddHandlerNoteCommand(
    string CaseNumber,
    string Message,
    bool IsInternal,
    int HandlerId,
    string? Attachment
) : IRequest<Result<int>>;