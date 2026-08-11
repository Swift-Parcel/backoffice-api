using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Customers.Queries.GetCustomerByEmail;

public record GetCustomerByEmailRequest(string Email);