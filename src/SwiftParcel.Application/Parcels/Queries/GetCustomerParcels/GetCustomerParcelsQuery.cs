using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;

namespace SwiftParcel.Application.Parcels.Queries.GetCustomerParcels;

public record GetCustomerParcelsQuery(string CustomerEmail) : IRequest<Result<List<CustomerParcelDto>>>;