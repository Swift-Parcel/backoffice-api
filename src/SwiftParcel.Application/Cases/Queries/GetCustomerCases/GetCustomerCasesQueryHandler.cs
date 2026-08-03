using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCustomerCases;

public class GetCustomerCasesQueryHandler : IRequestHandler<GetCustomerCasesQuery, Result<CustomerCasesResponse>>
{
    private readonly IAppDbContext _context;

    public GetCustomerCasesQueryHandler(IAppDbContext context) => _context = context;

    public async Task<Result<CustomerCasesResponse>> Handle(GetCustomerCasesQuery request, CancellationToken cancellationToken)
    {
        var cases = await _context.Cases
            .Include(c => c.Customer)
            .AsNoTracking()
            .Where(c => c.Customer.Email == request.CustomerEmail)
            .Select(c => new CustomerCaseItemDto(
                c.CaseNumber,
                c.CaseType,
                c.Status,
                c.CreatedDate,
                c.UpdatedDate
            ))
            .ToListAsync(cancellationToken);

        return Result<CustomerCasesResponse>.Success(new CustomerCasesResponse(cases));
    }
}