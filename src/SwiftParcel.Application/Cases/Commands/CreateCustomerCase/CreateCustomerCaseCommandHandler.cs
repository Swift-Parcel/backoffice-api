using MediatR;
using Microsoft.Extensions.Options;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Common.Settings;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Application.Services;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Commands.CreateCustomerCase;

public class CreateCustomerCaseCommandHandler 
    : IRequestHandler<CreateCustomerCaseCommand, Result<CreateCustomerCaseResponse>>
{
    private readonly ICaseRepository _caseRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IParcelRepository _parcelRepository;
    private readonly ICaseNumberGenerator _caseNumberGenerator;
    private readonly SlaOptions _slaOptions;
    private readonly IRegionRoutingService _regionRoutingService;

    public CreateCustomerCaseCommandHandler(
        ICaseRepository caseRepository,
        ICustomerRepository customerRepository,
        IParcelRepository parcelRepository,
        ICaseNumberGenerator caseNumberGenerator,
        IOptions<SlaOptions> slaOptions,
        IRegionRoutingService regionRoutingService)
    {
        _caseRepository = caseRepository;
        _customerRepository = customerRepository;
        _parcelRepository = parcelRepository;
        _caseNumberGenerator = caseNumberGenerator;
        _slaOptions = slaOptions.Value;
        _regionRoutingService = regionRoutingService;
    }

    public async Task<Result<CreateCustomerCaseResponse>> Handle(CreateCustomerCaseCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByEmailAsync(request.CustomerEmail, cancellationToken);

        if (customer == null)
        {
            return Result<CreateCustomerCaseResponse>.Failure(Error.NotFound(
                "create_customer_case__customer_not_found", 
                $"Customer with email '{request.CustomerEmail}' not found."));
        }

        var parcels = new List<Parcel>();
        if (request.TrackingNumbers.Any())
        {
            parcels = await _parcelRepository.GetByTrackingNumbersAsync(request.TrackingNumbers, cancellationToken);

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
            SlaDeadline = now.AddHours(slaHours),
            Channel = Channel.Portal,
            Parcels = parcels
        };

        newCase.RegionId = await _regionRoutingService.DetermineRegionAsync(newCase, cancellationToken);

        await _caseRepository.AddAsync(newCase, cancellationToken);

        return Result<CreateCustomerCaseResponse>.Success(
            new CreateCustomerCaseResponse(newCase.CaseNumber));
    }
}