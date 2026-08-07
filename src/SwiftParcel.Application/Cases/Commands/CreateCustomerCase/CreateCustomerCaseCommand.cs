using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.CreateCustomerCase;

public record CreateCustomerCaseCommand(
    string Title,
    string CustomerEmail,
    List<string> TrackingNumbers,
    CaseType CaseType,
    string Description) : IRequest<Result<CreateCustomerCaseResponse>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
};