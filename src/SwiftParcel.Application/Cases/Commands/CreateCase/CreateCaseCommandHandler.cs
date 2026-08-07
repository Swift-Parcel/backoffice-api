using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SwiftParcel.Application.Cases.Dtos;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Common.Settings;
using SwiftParcel.Application.Services;
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
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == request.CustomerEmail, cancellationToken);

        if (customer is null)
            return Result<CreateCaseResponse>.Failure(Error.NotFound("code",
                $"Customer with email '{request.CustomerEmail}' does not exist."));

        var activeRegionExists = await _context.Regions
            .AnyAsync(r => r.Id == request.RegionId && r.IsActive, cancellationToken);
        
        if (!activeRegionExists)
            return Result<CreateCaseResponse>.Failure(Error.NotFound("code", 
                $"Region with ID '{request.RegionId}' does not exist or is inactive."));

        var parcels = await _context.Parcels
            .Where(p => request.ParcelIds.Contains(p.Id))
            .ToListAsync(cancellationToken);
        
        if (parcels.Count != request.ParcelIds.Distinct().Count())
            return Result<CreateCaseResponse>.Failure(Error.NotFound("code", 
                "One or more specified Parcel IDs do not exist."));

        var tags = request.TagIds.Any()
            ? await _context.Tags.Where(t => request.TagIds.Contains(t.Id)).ToListAsync(cancellationToken)
            : new List<Tag>();
        
        if (request.TagIds.Any() && tags.Count != request.TagIds.Distinct().Count())
            return Result<CreateCaseResponse>.Failure(Error.NotFound("code", 
                "One or more specified Tag IDs do not exist."));
        
        string caseNumber = await _caseNumberGenerator.GenerateNextAsync(cancellationToken);
        int slaHours = _slaOptions.DefaultHours.GetValueOrDefault(request.CaseType, 72);

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
            SlaDeadline = now.AddHours(slaHours), // TODO: calculate actual deadline based on operating hours?
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