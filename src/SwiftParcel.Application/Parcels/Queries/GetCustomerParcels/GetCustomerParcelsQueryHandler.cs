using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Parcels.Queries.GetCustomerParcels;

public class GetCustomerParcelsQueryHandler : IRequestHandler<GetCustomerParcelsQuery, Result<List<CustomerParcelDto>>>
{
    private readonly IParcelRepository _parcelRepository;
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerParcelsQueryHandler(
        IParcelRepository parcelRepository,
        ICustomerRepository customerRepository)
    {
        _parcelRepository = parcelRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Result<List<CustomerParcelDto>>> Handle(GetCustomerParcelsQuery request, CancellationToken cancellationToken)
    {
        var customerExists = await _customerRepository.ExistsByEmailAsync(request.CustomerEmail, cancellationToken);

        if (!customerExists)
        {
            return Result<List<CustomerParcelDto>>.Failure(
                Error.NotFound("customer_not_found", $"Customer with email '{request.CustomerEmail}' was not found."));
        }

        var parcels = await _parcelRepository.GetCustomerParcelsByEmailAsync(request.CustomerEmail, cancellationToken);

        return Result<List<CustomerParcelDto>>.Success(parcels);
    }
}