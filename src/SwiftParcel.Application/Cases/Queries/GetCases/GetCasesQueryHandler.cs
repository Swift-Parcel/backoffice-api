using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;

namespace SwiftParcel.Application.Cases.Queries.GetCases;

public class GetCasesQueryHandler(IAppDbContext dbContext, ICurrentUserService currentUser)
    : IRequestHandler<GetCasesQuery, List<CaseDto>>
{
    public async Task<List<CaseDto>> Handle(GetCasesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Cases.AsNoTracking();

        if (!currentUser.CanAccessAllRegions)
        {
            var userRegions = currentUser.GetRegionIds();
            
            if (!userRegions.Any())
            {
                return new List<CaseDto>();
            }

            query = query.Where(c => userRegions.Contains(c.RegionId));
        }

        return await query
            .Select(c => new CaseDto
            {
                Id = c.Id,
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
        
                HandlerId = c.HandlerId,
                HandlerName = c.Handler != null ? c.Handler.User.FullName : null,

                Tags = c.Tags.Select(t => new TagDto 
                { 
                    Id = t.Id, 
                    Name = t.Name 
                }).ToList()
            })
            .ToListAsync(cancellationToken);
    }
}