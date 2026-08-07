using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCustomerCases;

public class GetCustomerCasesQueryHandler : IRequestHandler<GetCustomerCasesQuery, Result<CustomerCasesResponse>>
{
    private readonly ICaseRepository _caseRepository;

    public GetCustomerCasesQueryHandler(ICaseRepository caseRepository) => _caseRepository = caseRepository;

    public async Task<Result<CustomerCasesResponse>> Handle(
        GetCustomerCasesQuery request, CancellationToken cancellationToken)
    {
        var cases = await _caseRepository.GetCustomerCasesByEmailAsync(request.CustomerEmail, cancellationToken);

        return Result<CustomerCasesResponse>.Success(new CustomerCasesResponse(cases));
    }
}