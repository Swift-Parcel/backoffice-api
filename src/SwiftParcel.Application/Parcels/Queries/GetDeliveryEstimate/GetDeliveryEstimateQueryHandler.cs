using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Services;
using SwiftParcel.Domain.Shared;
using SwiftParcel.Domain.ValueObjects; // Ezt hozzá kell adni!

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
        var trackingNumberResult = TrackingNumber.Create(request.TrackingNumber);

        if (!trackingNumberResult.IsSuccess)
        {
            return Result<DeliveryEstimateResponse>.Failure(Error.Validation("code","Invalid tracking number."));
        }

        var trackingNumber = trackingNumberResult.Value;

        
        var parcelExists = await _parcelRepository.ExistsByTrackingNumberAsync(trackingNumber, cancellationToken);

        if (!parcelExists)
        {
            return Result<DeliveryEstimateResponse>.Failure(
                Error.NotFound("parcel_not_found", $"Parcel with tracking number '{trackingNumber.Value}' was not found."));
        }

        var estimate = await _estimationService.CalculateForParcelAsync(trackingNumber, cancellationToken);

        if (estimate == null)
        {
            return Result<DeliveryEstimateResponse>.Failure(
                Error.Failure("estimate_calculation_failed", "Could not calculate delivery estimate."));
        }

        return Result<DeliveryEstimateResponse>.Success(estimate);
    }
}