using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.AddCaseNote;

public record AddHandlerNoteCommand(
    string CaseNumber,
    string Message,
    bool IsInternal,
    string? Attachment
) : IRequest<Result<int>>;