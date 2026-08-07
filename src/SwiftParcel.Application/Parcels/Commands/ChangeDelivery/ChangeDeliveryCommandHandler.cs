using MediatR;
using Microsoft.Extensions.Options;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Common.Settings;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Services;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Parcels.Commands.ChangeDelivery;

public class ChangeDeliveryCommandHandler : IRequestHandler<ChangeDeliveryCommand, Result<DeliveryChangeResponse>>
{
    private readonly IParcelRepository _parcelRepository;
    private readonly ICaseRepository _caseRepository;
    private readonly IRegionRoutingService _regionRoutingService;
    private readonly ICaseNumberGenerator _caseNumberGenerator;
    private readonly SlaOptions _slaOptions;

    public ChangeDeliveryCommandHandler(
        IParcelRepository parcelRepository,
        IRegionRepository regionRepository,
        ICaseRepository caseRepository,
        IRegionRoutingService regionRoutingService,
        ICaseNumberGenerator caseNumberGenerator,
        IOptions<SlaOptions> slaOptions)
    {
        _parcelRepository = parcelRepository;
        _caseRepository = caseRepository;
        _regionRoutingService = regionRoutingService;
        _caseNumberGenerator = caseNumberGenerator;
        _slaOptions = slaOptions.Value;
    }

    public async Task<Result<DeliveryChangeResponse>> Handle(ChangeDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        var parcel = await _parcelRepository.GetByTrackingNumberAsync(request.TrackingNumber, cancellationToken);

        if (parcel == null)
            return Result<DeliveryChangeResponse>.Failure(Error.NotFound("parcel_not_found",
                $"Parcel with tracking number '{request.TrackingNumber}' was not found."));

        if (parcel.Customer?.Address == null)
        {
            return Result<DeliveryChangeResponse>.Failure(
                Error.Failure("customer_address_missing", "Customer does not have a valid address."));
        }

        var countryCode = parcel.Customer.Address.CountryCode;
        var regionId = await _regionRoutingService.DetermineRegionAsync(parcel, cancellationToken);
        
        var caseNumber = await _caseNumberGenerator.GenerateNextAsync(cancellationToken);
        var slaHours = _slaOptions.DefaultHours.GetValueOrDefault(CaseType.DeliveryChange, 72);
        var newCase = Case.CreateForDeliveryChange(
            caseNumber: caseNumber,
            customer: parcel.Customer,
            parcel: parcel,
            regionId: regionId,
            newDate: request.Date,
            newTimeslot: request.Timeslot,
            slaHours: slaHours
        );

        await _caseRepository.AddAsync(newCase, cancellationToken);

        return Result<DeliveryChangeResponse>.Success(new DeliveryChangeResponse(newCase.CaseNumber));
    }
}