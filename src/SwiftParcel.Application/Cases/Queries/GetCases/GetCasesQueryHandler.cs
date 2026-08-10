using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Application.DTO.Tags;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Queries.GetCases;

public class GetCasesQueryHandler(ICaseRepository caseRepository, ICurrentUserService currentUser)
    : IRequestHandler<GetCasesQuery, Result<PagedList<CaseDto>>>
{
    public async Task<Result<PagedList<CaseDto>>> Handle(GetCasesQuery request, CancellationToken cancellationToken)
    {
        var pagedEntities = await caseRepository.GetCasesFilteredPagedAsync(
            allowedRegionIds: null,
            canAccessAllRegions: false,
            customerId: request.CustomerId,
            customerEmail: request.CustomerEmail,
            customerPhone: request.CustomerPhone,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            cancellationToken);

        var dtos = pagedEntities.Items.Select(c => new CaseDto
        {
            CaseNumber = c.CaseNumber,
            Title = c.Title,
            Description = c.Description,
            CaseType = c.CaseType,
            Status = c.Status,
            Priority = c.Priority,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate,
            IsEscalated = c.IsEscalated,
            ResolvedDate = c.ResolvedDate,
            SlaDeadline = c.SlaDeadline,
            Channel = c.Channel,
            Resolution = c.Resolution,
            SatisfactionScore = c.SatisfactionScore,
            CustomerId = c.CustomerId,
            CustomerName = c.Customer.FullName,
            RegionId = c.RegionId,
            RegionName = c.Region.Name,
            Tags = c.Tags.Select(t => new TagDto
            (
                Id: t.Id,
                Name: t.Name
            )).ToList()
        }).ToList();

        var pagedDtos = new PagedList<CaseDto>(
            dtos,
            pagedEntities.TotalCount,
            pagedEntities.PageNumber,
            pagedEntities.PageSize);

        return Result<PagedList<CaseDto>>.Success(pagedDtos);
    }
}