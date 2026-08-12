using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Shared;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Queries.GetCases;

public record GetCasesQuery(
    int? CustomerId = null,
    string? CustomerEmail = null,
    string? CustomerPhone = null,
    string? SearchTerm = null
) : PagedQuery, IRequest<Result<PagedList<CaseDto>>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.ReadOnly, UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
};