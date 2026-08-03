using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Cases.Commands.CreateCase;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Parcels.Commands.ChangeDelivery;

public class ChangeDeliveryCommandHandler : IRequestHandler<ChangeDeliveryCommand, Result<DeliveryChangeResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ISender _mediator;

    public ChangeDeliveryCommandHandler(IAppDbContext context, ISender mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<DeliveryChangeResponse>> Handle(ChangeDeliveryCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _context.Parcels
            .Include(p => p.Customer)
            .FirstOrDefaultAsync(p => p.TrackingNumber == request.TrackingNumber, cancellationToken);

        if (parcel == null)
            return Result<DeliveryChangeResponse>.Failure(Error.NotFound("parcel_not_found", $"Parcel with tracking number '{request.TrackingNumber}' was not found."));

        var countryCode = await _context.Customers
            .Where(c => c.Id == parcel.CustomerId)
            .Select(c => c.Address.CountryCode)
            .FirstOrDefaultAsync(cancellationToken);

        var regionId = await _context.Regions
            .Where(r => r.CountryCode == countryCode && r.IsActive)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(cancellationToken);

        bool vip = parcel.Customer.Vip;
        
        var createCaseCommand = new CreateCaseCommand(
            Title: "Delivery Change",
            Description: $"{request.Date} - {request.Timeslot}",
            CaseType: CaseType.DeliveryChange,
            CaseStatus: CaseStatus.Open,
            Priority: vip ? Priority.High : Priority.Low,
            CustomerEmail: parcel.Customer.Email,
            RegionId: regionId,
            Channel: Channel.Portal,
            TagIds: Array.Empty<int>(),
            ParcelIds: new[] { parcel.Id }
        );

        var caseResult = await _mediator.Send(createCaseCommand, cancellationToken);

        if (!caseResult.IsSuccess)
            return Result<DeliveryChangeResponse>.Failure(caseResult.Error);

        return Result<DeliveryChangeResponse>.Success(new DeliveryChangeResponse(caseResult.Value.ToString()));
    }
}