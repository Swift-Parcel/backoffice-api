using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Customers.Queries.GetCustomerByEmail;

public class GetCustomerByEmailQueryHandler(ICustomerRepository customerRepository)
    : IRequestHandler<GetCustomerByEmailQuery, Result<CustomerDetailsDto>>
{
    public async Task<Result<CustomerDetailsDto>> Handle(GetCustomerByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (customer == null)
        {
            return Result<CustomerDetailsDto>.Failure(
                Error.NotFound($"Customer with email '{request.Email}' was not found."));
        }

        var dto = new CustomerDetailsDto
        (
            Email: customer.Email!,
            FullName: customer.FullName,
            Phone: customer.Phone,
            RegisteredDate: customer.RegisteredDate,
            Vip: customer.Vip,
            Notes: customer.Notes,
            Address: new AddressDto(
                City: customer.Address.City,
                CountryCode: customer.Address.CountryCode,
                PostalCode: customer.Address.PostalCode,
                Street: customer.Address.Street,
                StreetNumber: customer.Address.StreetNumber)
        );
        
        return Result<CustomerDetailsDto>.Success(dto);
    }
}