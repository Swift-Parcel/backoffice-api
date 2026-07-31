using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using SwiftParcel.Application.DTO.Webhooks;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Infrastructure.Services;

public class WebhookClient : IWebhookClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebhookClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public WebhookClient(HttpClient httpClient, ILogger<WebhookClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        _jsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task NotifyParcelStatusChangedAsync(string trackingNumber, ParcelStatus status, CancellationToken cancellationToken = default)
    {
        var payload = new ParcelStatusWebhookRequest(trackingNumber, status);
        await SendWebhookAsync("/api/webhooks/parcels/status", payload, cancellationToken);
    }

    public async Task NotifyCaseStatusChangedAsync(string caseNumber, CaseStatus status, CancellationToken cancellationToken = default)
    {
        var payload = new CaseStatusWebhookRequestDto(caseNumber, status);
        await  SendWebhookAsync("/api/webhooks/case/status", payload, cancellationToken);
    }

    private async Task SendWebhookAsync<T>(string endpoint, T payload, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, payload, _jsonOptions, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Webhook call to {endpoint} failed with status code {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while sending webhook to {endpoint}");
        }
    }
}