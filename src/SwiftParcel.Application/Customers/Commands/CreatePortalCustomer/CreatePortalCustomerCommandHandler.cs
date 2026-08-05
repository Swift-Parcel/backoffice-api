using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Customers;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Customers.Commands.CreatePortalCustomer;

public class CreatePortalCustomerCommandHandler
    : IRequestHandler<CreatePortalCustomerCommand, Result<CreateCustomerResponse>>
{
    private readonly IAppDbContext _context;

    public CreatePortalCustomerCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateCustomerResponse>> Handle(CreatePortalCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var emailExists = await _context.Customers
            .AnyAsync(c => c.Email == request.Email, cancellationToken);

        if (emailExists)
        {
            return Result<CreateCustomerResponse>.Failure(
                Error.Failure("Customer.EmailAlreadyInUse", "A customer already has this email."));
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

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CreateCustomerResponse>.Success(
            new CreateCustomerResponse(customer.RegisteredDate));
    }
}