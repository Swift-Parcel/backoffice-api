using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Services;

namespace SwiftParcel.Application.Parcels.Queries.GetDeliveryEstimate;

public class GetDeliveryEstimateQueryHandler : IRequestHandler<GetDeliveryEstimateQuery, Result<DeliveryEstimateResponse>>
{
    private readonly IParcelRepository _parcelRepository;
    private readonly IDeliveryEstimationService _estimationService;

    public GetDeliveryEstimateQueryHandler(
        IParcelRepository parcelRepository, 
        IDeliveryEstimationService estimationService)
    {
        _parcelRepository = parcelRepository;
        _estimationService = estimationService;
    }

    public async Task<Result<DeliveryEstimateResponse>> Handle(GetDeliveryEstimateQuery request, CancellationToken cancellationToken)
    {
        var parcelExists = await _parcelRepository.ExistsByTrackingNumberAsync(request.TrackingNumber, cancellationToken);

        if (!parcelExists)
        {
            return Result<DeliveryEstimateResponse>.Failure(
                Error.NotFound("parcel_not_found", $"Parcel with tracking number '{request.TrackingNumber}' was not found."));
        }

        var estimate = await _estimationService.CalculateForParcelAsync(request.TrackingNumber, cancellationToken);

        if (estimate == null)
        {
            return Result<DeliveryEstimateResponse>.Failure(
                Error.Failure("estimate_calculation_failed", "Could not calculate delivery estimate."));
        }

        return Result<DeliveryEstimateResponse>.Success(estimate);
    }
}