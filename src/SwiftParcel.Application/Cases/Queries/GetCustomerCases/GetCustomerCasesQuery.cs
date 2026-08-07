using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Shared;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Queries.GetCustomerCases;

public record GetCustomerCasesQuery(string CustomerEmail) : IRequest<Result<CustomerCasesResponse>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
};