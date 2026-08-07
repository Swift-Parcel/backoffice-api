using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCases;

public class GetCasesQueryHandler(IAppDbContext dbContext, ICurrentUserService currentUser) : IRequestHandler<GetCasesQuery, Result<List<CaseDto>>>
{
    public async Task<Result<List<CaseDto>>> Handle(GetCasesQuery request, CancellationToken cancellationToken)    
    {
        var query = dbContext.Cases.AsNoTracking();

        if (!currentUser.CanAccessAllRegions)
        {
            var userRegions = currentUser.GetRegionIds();
            
            if (!userRegions.Any())
            {
                return Result<List<CaseDto>>.Success(new List<CaseDto>());
            }

            query = query.Where(c => userRegions.Contains(c.RegionId));
        }

        if (request.CustomerId.HasValue)
        {
            query = query.Where(c => c.CustomerId == request.CustomerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
        {
            query = query.Where(c => c.Customer.Email.ToLower() == request.CustomerEmail.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(request.CustomerPhone))
        {
            query = query.Where(c => c.Customer.Phone == request.CustomerPhone);
        }

        var cases = await query
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

        return Result<List<CaseDto>>.Success(cases);
    }
}