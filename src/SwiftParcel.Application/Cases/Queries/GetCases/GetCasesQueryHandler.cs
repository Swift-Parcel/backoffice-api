using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCases;

public class GetCasesQueryHandler(ICaseRepository caseRepository, ICurrentUserService currentUser) 
    : IRequestHandler<GetCasesQuery, Result<List<CaseDto>>>
{
    public async Task<Result<List<CaseDto>>> Handle(GetCasesQuery request, CancellationToken cancellationToken)    
    {
        var cases = await caseRepository.GetFilteredCasesAsync(
            allowedRegionIds: currentUser.GetRegionIds(),
            canAccessAllRegions: currentUser.CanAccessAllRegions,
            customerId: request.CustomerId,
            customerEmail: request.CustomerEmail,
            customerPhone: request.CustomerPhone,
            cancellationToken: cancellationToken);

        return Result<List<CaseDto>>.Success(cases);
    }
}