using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SwiftParcel.Application.Common.Interfaces.Webhooks;
using SwiftParcel.Application.DTO.Webhooks;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Infrastructure.Services;

public class CustomerPortalNotificationService : ICustomerPortalNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomerPortalNotificationService> _logger;

    public CustomerPortalNotificationService(HttpClient httpClient, ILogger<CustomerPortalNotificationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task NotifyParcelStatusChangedAsync(string trackingNumber, ParcelStatus newStatus, CancellationToken cancellationToken = default)
    {
        var payload = new ParcelStatusNotificationDto(trackingNumber, newStatus.ToString());
        await SendNotificationAsync("api/webhooks/parcels/status", payload, cancellationToken);
    }

    public async Task NotifyCaseStatusChangedAsync(string caseNumber, CaseStatus newStatus, string? resolution, CancellationToken cancellationToken = default)
    {
        var payload = new CaseStatusNotificationDto(caseNumber, newStatus.ToString(), resolution);
        await SendNotificationAsync("api/webhooks/cases/status", payload, cancellationToken);
    }

    public async Task NotifyDeliveryChangeResolvedAsync(string caseNumber, DeliveryChangeOutcome outcome, CancellationToken cancellationToken = default)
    {
        var payload = new DeliveryChangeNotificationDto(caseNumber, outcome.ToString());
        await SendNotificationAsync("api/webhooks/cases/delivery-change", payload, cancellationToken);
    }

    private async Task SendNotificationAsync<T>(string endpoint, T payload, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to notify Customer Portal on endpoint '{Endpoint}'. Status Code: {ResponseStatusCode}", endpoint, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending notification to Customer Portal on endpoint '{Endpoint}'", endpoint);
        }
    }
}