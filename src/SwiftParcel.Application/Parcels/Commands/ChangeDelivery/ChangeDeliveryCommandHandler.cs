using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Cases.Commands.CreateCase;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Common.Settings;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Parcels.Commands.ChangeDelivery;

public class ChangeDeliveryCommandHandler : IRequestHandler<ChangeDeliveryCommand, Result<DeliveryChangeResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICaseNumberGenerator _caseNumberGenerator;
    private readonly SlaOptions _slaOptions;

    public ChangeDeliveryCommandHandler(
        IAppDbContext context,
        ICaseNumberGenerator caseNumberGenerator,
        SlaOptions slaOptions)
    {
        _context = context;
        _caseNumberGenerator = caseNumberGenerator;
        _slaOptions = slaOptions;
    }

    public async Task<Result<DeliveryChangeResponse>> Handle(ChangeDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        var parcel = await _context.Parcels
            .Include(p => p.Customer)
            .ThenInclude(c => c.Address)
            .FirstOrDefaultAsync(p => p.TrackingNumber == request.TrackingNumber, cancellationToken);

        if (parcel == null)
            return Result<DeliveryChangeResponse>.Failure(Error.NotFound("parcel_not_found",
                $"Parcel with tracking number '{request.TrackingNumber}' was not found."));


        if (parcel.Customer?.Address == null)
        {
            return Result<DeliveryChangeResponse>.Failure(
                Error.Failure("customer_address_missing", "Customer does not have a valid address."));
        }

        var countryCode = parcel.Customer.Address.CountryCode;
        var regionId = await _context.Regions
            .Where(r => r.CountryCode == countryCode && r.IsActive)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(cancellationToken);


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


        _context.Cases.Add(newCase);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<DeliveryChangeResponse>.Success(new DeliveryChangeResponse(newCase.CaseNumber));
    }
}