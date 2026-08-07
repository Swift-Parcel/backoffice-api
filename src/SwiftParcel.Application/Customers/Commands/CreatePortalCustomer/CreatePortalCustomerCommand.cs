using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Customers;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Customers.Commands.CreatePortalCustomer;

public record CreatePortalCustomerCommand(
    string Name,
    string Email,
    string Phone,
    AddressDto Address)
    : IRequest<Result<CreateCustomerResponse>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Supervisor, UserRole.Admin];
};