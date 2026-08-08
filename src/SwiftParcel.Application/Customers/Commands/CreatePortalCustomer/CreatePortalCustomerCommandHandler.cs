using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Customers;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Customers.Commands.CreatePortalCustomer;

public class CreatePortalCustomerCommandHandler
    : IRequestHandler<CreatePortalCustomerCommand, Result<CreateCustomerResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public CreatePortalCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CreateCustomerResponse>> Handle(CreatePortalCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var emailExists = await _customerRepository.ExistsByEmailAsync(request.Email, cancellationToken);

        if (emailExists)
        {
            return Result<CreateCustomerResponse>.Failure(
                Error.Failure("A customer already has this email."));
        }

        var customer = new Customer
        {
            FullName = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            RegisteredDate = DateTime.UtcNow,
            Vip = false,
            Address = new Address
            (
                request.Address.Street.Trim(),
                request.Address.StreetNumber.Trim(),
                request.Address.City.Trim(),
                request.Address.PostalCode.Trim(),
                request.Address.CountryCode.Trim().ToUpper()
            )
        };

        await _customerRepository.AddAsync(customer, cancellationToken);

        return Result<CreateCustomerResponse>.Success(
            new CreateCustomerResponse(customer.RegisteredDate));
    }
}