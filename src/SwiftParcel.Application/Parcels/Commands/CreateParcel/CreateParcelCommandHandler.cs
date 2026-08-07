using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;
using SwiftParcel.Domain.ValueObjects;

namespace SwiftParcel.Application.Parcels.Commands.CreateParcel;

public class CreateParcelCommandHandler : IRequestHandler<CreateParcelCommand, Result<CreateParcelResponse>>
{
    private readonly IParcelRepository _parcelRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IWebhookClient _webhookClient;
    private readonly IParcelNumberGenerator _parcelNumberGenerator;
    private readonly ILogger<CreateParcelCommandHandler> _logger;

    public CreateParcelCommandHandler(
        IParcelRepository parcelRepository,
        ICustomerRepository customerRepository,
        IWebhookClient webhookClient, 
        IParcelNumberGenerator parcelNumberGenerator,
        ILogger<CreateParcelCommandHandler> logger)
    {
        _parcelRepository = parcelRepository;
        _customerRepository = customerRepository;
        _webhookClient = webhookClient;
        _parcelNumberGenerator = parcelNumberGenerator;
        _logger = logger;
    }

    public async Task<Result<CreateParcelResponse>> Handle(CreateParcelCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByEmailAsync(request.Sender.Email, cancellationToken);

        if (customer == null)
        {
            return Result<CreateParcelResponse>.Failure(
                Error.NotFound("customer_not_found", $"Customer with email '{request.Sender.Email}' was not found."));
        }

        var now = DateTime.UtcNow;
        var trackingNumber = await _parcelNumberGenerator.GenerateUniqueCodeAsync(cancellationToken);

        var newParcel = new Parcel
        {
            TrackingNumber = TrackingNumber.Create(trackingNumber).Value,
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
            PreferredPickupDate = request.Parcel.PreferredPickupDate,
            PreferredPickupTimeslot = request.Parcel.PreferredPickupTimeslot,
            RecipientAddress = new Address(
                request.Recipient.RecipientAddress.Street,
                request.Recipient.RecipientAddress.StreetNumber,
                request.Recipient.RecipientAddress.City,
                request.Recipient.RecipientAddress.PostalCode,
                request.Recipient.RecipientAddress.CountryCode
            )
            ,
            SenderAddress = new Address(
                request.Sender.SenderAddress.Street,
                request.Sender.SenderAddress.StreetNumber,
                request.Sender.SenderAddress.City,
                request.Sender.SenderAddress.PostalCode,
                request.Sender.SenderAddress.CountryCode
            )
        };

        await _parcelRepository.AddAsync(newParcel, cancellationToken);

        try
        {
            await _webhookClient.NotifyParcelStatusChangedAsync(newParcel.TrackingNumber, newParcel.Status, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch webhook notification for newly created Parcel: {TrackingNumber}", newParcel.TrackingNumber);
        }

        return Result<CreateParcelResponse>.Success(new CreateParcelResponse(newParcel.TrackingNumber, newParcel.Status));
    }
}