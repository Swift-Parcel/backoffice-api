using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Commands.AddCaseNote;

public record AddCustomerNoteCommand(
    string CaseNumber,
    string Message,
    string CustomerEmail,
    string? Attachment
) : IRequest<Result<int>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
};