using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SwiftParcel.Application.Cases.Dtos;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Common.Settings;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Cases.Commands.CreateCase;

public class CreateCaseCommandHandler 
    : IRequestHandler<CreateCaseCommand, Result<CreateCaseResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICaseNumberGenerator _caseNumberGenerator;
    private readonly SlaOptions _slaOptions;
    private readonly IRegionRoutingService _regionRoutingService;

    public CreateCaseCommandHandler(
        IAppDbContext context, 
        ICaseNumberGenerator caseNumberGenerator,
        IOptions<SlaOptions> slaOptions,
        IRegionRoutingService regionRoutingService)
    {
        _context = context;
        _caseNumberGenerator = caseNumberGenerator;
        _slaOptions = slaOptions.Value;
        _regionRoutingService = regionRoutingService;
    }
    
    public async Task<Result<CreateCaseResponse>> Handle(CreateCaseCommand request,
        CancellationToken cancellationToken)
    {
        var parcels = await _context.Parcels
            .Where(p => request.ParcelIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var existingParcelIds = parcels.Select(p => p.Id).ToList();

        var missingParcelIds = request.ParcelIds
            .Except(existingParcelIds)
            .ToList();

        if (missingParcelIds.Any())
        {
            var missingIdsString = string.Join(", ", missingParcelIds);
    
            return Result<CreateCaseResponse>.Failure(Error.Validation(
                "create_case__missing_parcels", 
                $"The following ParcelIds do not exist: {missingIdsString}"));
        }
        
        string caseNumber = await _caseNumberGenerator.GenerateNextAsync(cancellationToken);
        
        var customer = await _context.Customers.FirstOrDefaultAsync(c 
            => c.Email == request.CustomerEmail, cancellationToken);

        if (customer == null)
        {
            return Result<CreateCaseResponse>.Failure(Error.NotFound(
                "create_case__customer_not_found", 
                $"Customer with email '{request.CustomerEmail}' not found."));
        }
        
        
        List<Tag> tags = new();
    
        if (request.TagIds.Count > 0 )
        {
            tags = await _context.Tags
                .Where(t => request.TagIds.Contains(t.Id))
                .ToListAsync(cancellationToken);
            if (tags.Count != request.TagIds.Count)
            {
                var missingTagIds = request.TagIds
                    .Except(tags.Select(t => t.Id));
                
                return Result<CreateCaseResponse>.Failure(Error.Validation("code",
                    $"The following tags aren't present in the database:{string.Join(", ", missingTagIds)}"));
            }
        }

        int slaHours = _slaOptions.DefaultHours
            .GetValueOrDefault(request.CaseType, 72);

        DateTime now = DateTime.UtcNow;
        
        var newCase = new Case
        {
            CaseNumber = caseNumber,
            Title = request.Title,
            Description = request.Description,
            CaseType = request.CaseType,
            Status = request.CaseStatus,
            Priority = request.Priority,
            Customer = customer,
            HandlerId = request.HandlerId,
            CreatedDate = now,
            SlaDeadline = now.AddHours(slaHours),
            Channel = request.Channel,
            Tags = tags,
            Parcels = parcels
        };
        
        newCase.RegionId = request.RegionId ?? await _regionRoutingService.DetermineRegionAsync(newCase, cancellationToken);
        
        _context.Cases.Add(newCase);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result<CreateCaseResponse>.Success(new 
            CreateCaseResponse(newCase.CaseNumber));
    }
}