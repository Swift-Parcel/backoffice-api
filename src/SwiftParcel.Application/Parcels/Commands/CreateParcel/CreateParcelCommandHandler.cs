using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Parcels.Commands.CreateParcel;

public class CreateParcelCommandHandler : IRequestHandler<CreateParcelCommand, Result<CreateParcelResponse>>
{
    private readonly IAppDbContext _context;
    private readonly IWebhookClient _webhookClient;

    public CreateParcelCommandHandler(IAppDbContext context, IWebhookClient webhookClient)
    {
        _context = context;
        _webhookClient = webhookClient;
    }

    public async Task<Result<CreateParcelResponse>> Handle(CreateParcelCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == request.Sender.Email, cancellationToken);

        if (customer == null)
        {
            return Result<CreateParcelResponse>.Failure(
                Error.NotFound("customer_not_found", $"Customer with email '{request.Sender.Email}' was not found."));
        }

        var now = DateTime.UtcNow;
        
        var prefix = $"SP-{now:yyyyMM}";
        var parcelsThisMonth = await _context.Parcels
            .CountAsync(p => p.CreatedDate.Year == now.Year && p.CreatedDate.Month == now.Month, cancellationToken);
        var trackingNumber = $"{prefix}{(parcelsThisMonth + 1):D2}";

        var newParcel = new Parcel
        {
            TrackingNumber = trackingNumber,
            CustomerId = customer.Id,
            RecipientName = request.Recipient.Name,
            Weight = request.Parcel.Weight,
            Width = request.Parcel.Width,
            Length = request.Parcel.Length,
            Height = request.Parcel.Height,
            ServiceType = request.Parcel.ServiceType,
            DeclaredValueInEuros = request.Parcel.DeclaredValue,
            Status = ParcelStatus.PendingPickup,
            CreatedDate = now,
            RecipientAddress = new Address(
                request.Recipient.RecipientAddress.Street,
                request.Recipient.RecipientAddress.StreetNumber,
                request.Recipient.RecipientAddress.City,
                request.Recipient.RecipientAddress.PostalCode,
                request.Recipient.RecipientAddress.CountryCode
            )
        };

        _context.Parcels.Add(newParcel);
        await _context.SaveChangesAsync(cancellationToken);

        await _webhookClient.NotifyParcelStatusChangedAsync(newParcel.TrackingNumber, newParcel.Status, cancellationToken);

        return Result<CreateParcelResponse>.Success(new CreateParcelResponse(newParcel.TrackingNumber, newParcel.Status));
    }
}