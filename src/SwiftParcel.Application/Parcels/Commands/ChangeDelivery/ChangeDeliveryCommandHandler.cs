using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Parcels.Commands.ChangeDelivery;

public class ChangeDeliveryCommandHandler : IRequestHandler<ChangeDeliveryCommand, Result<DeliveryChangeResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICaseService _caseService;
    private readonly IWebhookClient _webhookClient;

    public ChangeDeliveryCommandHandler(IAppDbContext context, ICaseService caseService, IWebhookClient webhookClient)
    {
        _context = context;
        _caseService = caseService;
        _webhookClient = webhookClient;
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

        var caseRequest = new CreateCaseRequest(
            CustomerEmail: parcel.Customer.Email,
            TrackingNumbers: [request.TrackingNumber],
            CaseType: CaseType.DeliveryChange,
            CaseTitle: "Delivery Change",
            RegionId: regionId,
            Channel: Channel.Portal,
            Description: $"{request.Date} - {request.Timeslot}"
        );

        var caseResponse = await _caseService.CreateCaseAsync(caseRequest, cancellationToken);

        if (caseResponse is null)
            return Result<DeliveryChangeResponse>.Failure(Error.Failure("create_case_failed", "Failed to create delivery change case."));

        await _webhookClient.NotifyDeliveryChangeOutcomeAsync(
            caseResponse.CaseNumber,
            DeliveryChangeOutcome.Approved,
            cancellationToken);

        return Result<DeliveryChangeResponse>.Success(new DeliveryChangeResponse(caseResponse.CaseNumber));
    }
}