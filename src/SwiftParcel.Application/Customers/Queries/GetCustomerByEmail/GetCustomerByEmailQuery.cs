using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Customers.Queries.GetCustomerByEmail;

public record GetCustomerByEmailQuery(string Email) : IRequest<Result<CustomerDetailsDto>>, IAuthorizableRequest
{
    public bool RequireAuthentication = true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Admin, UserRole.Operator, UserRole.Supervisor];
}   