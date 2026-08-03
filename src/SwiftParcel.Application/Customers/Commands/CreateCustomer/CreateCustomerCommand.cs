using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Customers;

namespace SwiftParcel.Application.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(
    string FullName,
    string Email,
    string Phone,
    AddressDto AddressDto)
    : IRequest<Result<CreateCustomerResponse>>;