using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;

namespace SwiftParcel.Application.Parcels.Queries.GetParcelStatus;

public record GetParcelStatusQuery(string TrackingNumber) : IRequest<Result<ParcelStatusResponse>>;