using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Queries.GetCaseNotes;

public record GetCaseNotesQuery(string CaseNumber) : IRequest<Result<IReadOnlyList<CaseNoteDto>>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
};