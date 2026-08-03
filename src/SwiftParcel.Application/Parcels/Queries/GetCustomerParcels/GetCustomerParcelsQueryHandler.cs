using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Parcels;

namespace SwiftParcel.Application.Parcels.Queries.GetCustomerParcels;

public class GetCustomerParcelsQueryHandler : IRequestHandler<GetCustomerParcelsQuery, Result<List<CustomerParcelDto>>>
{
    private readonly IAppDbContext _context;

    public GetCustomerParcelsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CustomerParcelDto>>> Handle(GetCustomerParcelsQuery request, CancellationToken cancellationToken)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.Email == request.CustomerEmail, cancellationToken);

        if (!customerExists)
        {
            return Result<List<CustomerParcelDto>>.Failure(
                Error.NotFound("customer_not_found", $"Customer with email '{request.CustomerEmail}' was not found."));
        }

        var parcels = await _context.Parcels
            .Where(p => p.Customer.Email == request.CustomerEmail)
            .Select(p => new CustomerParcelDto(
                p.TrackingNumber,
                p.Status,
                new CustomerParcelSenderDto(
                    p.Customer.Email,
                    new AddressDto(
                        p.Customer.Address.City,
                        p.Customer.Address.CountryCode,
                        p.Customer.Address.PostalCode,
                        p.Customer.Address.Street,
                        p.Customer.Address.StreetNumber
                    )
                ),
                new CustomerParcelRecipientDto(
                    p.RecipientName,
                    new AddressDto(
                        p.RecipientAddress.City,
                        p.RecipientAddress.CountryCode,
                        p.RecipientAddress.PostalCode,
                        p.RecipientAddress.Street,
                        p.RecipientAddress.StreetNumber
                    )
                ),
                p.CreatedDate,
                p.ServiceType
            ))
            .ToListAsync(cancellationToken);

        return Result<List<CustomerParcelDto>>.Success(parcels);
    }
}