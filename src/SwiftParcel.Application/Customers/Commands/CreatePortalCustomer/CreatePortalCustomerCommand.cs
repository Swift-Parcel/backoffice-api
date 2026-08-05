using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Customers;

namespace SwiftParcel.Application.Customers.Commands.CreatePortalCustomer;

public record CreatePortalCustomerCommand(
    string Name,
    string Email,
    string Phone,
    AddressDto Address)
    : IRequest<Result<CreateCustomerResponse>>;