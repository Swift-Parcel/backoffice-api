using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Common.Settings;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Commands.CreateCustomerCase;

public class CreateCustomerCaseCommandHandler 
    : IRequestHandler<CreateCustomerCaseCommand, Result<CreateCustomerCaseResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICaseNumberGenerator _caseNumberGenerator;
    private readonly SlaOptions _slaOptions;
    private readonly IRegionRoutingService _regionRoutingService;

    public CreateCustomerCaseCommandHandler(
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

    public async Task<Result<CreateCustomerCaseResponse>> Handle(CreateCustomerCaseCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c 
            => c.Email == request.CustomerEmail, cancellationToken);

        if (customer == null)
        {
            return Result<CreateCustomerCaseResponse>.Failure(Error.NotFound(
                "create_customer_case__customer_not_found", 
                $"Customer with email '{request.CustomerEmail}' not found."));
        }

        var parcels = new List<Parcel>();
        if (request.TrackingNumbers.Any())
        {
            parcels = await _context.Parcels
                .Where(p => request.TrackingNumbers.Contains(p.TrackingNumber))
                .ToListAsync(cancellationToken);

            var existingTrackingNumbers = parcels.Select(p => p.TrackingNumber).ToList();
            var missingTrackingNumbers = request.TrackingNumbers.Except(existingTrackingNumbers).ToList();

            if (missingTrackingNumbers.Any())
            {
                return Result<CreateCustomerCaseResponse>.Failure(Error.Validation(
                    "create_customer_case__invalid_tracking", 
                    $"The following tracking numbers do not exist: {string.Join(", ", missingTrackingNumbers)}"));
            }
        }

        string caseNumber = await _caseNumberGenerator.GenerateNextAsync(cancellationToken);
        int slaHours = _slaOptions.DefaultHours.GetValueOrDefault(request.CaseType, 72);
        DateTime now = DateTime.UtcNow;

        var newCase = new Case
        {
            CaseNumber = caseNumber,
            Title = request.Title,
            Description = request.Description,
            CaseType = request.CaseType,
            Status = CaseStatus.Open,
            Priority = Priority.Low,
            Customer = customer,
            CreatedDate = now,
            SlaDeadline = now.AddHours(slaHours), // TODO: calculate actual deadline based on operating hours?
            Channel = Channel.Portal,
            Parcels = parcels
        };

        newCase.RegionId = await _regionRoutingService.DetermineRegionAsync(newCase, cancellationToken);

        _context.Cases.Add(newCase);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CreateCustomerCaseResponse>.Success(
            new CreateCustomerCaseResponse(newCase.CaseNumber));
    }
}