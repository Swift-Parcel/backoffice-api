using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Parcels.Queries.GetCustomerParcels;

public record GetCustomerParcelsQuery(string CustomerEmail)
    : IRequest<Result<List<CustomerParcelDto>>>
{
};