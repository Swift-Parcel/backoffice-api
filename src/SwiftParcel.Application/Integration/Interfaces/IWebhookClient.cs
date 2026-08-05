using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Integration.Interfaces;

public interface IWebhookClient
{
    Task NotifyParcelStatusChangedAsync(string trackingNumber, ParcelStatus status, CancellationToken cancellationToken = default);
    Task NotifyCaseStatusChangedAsync(string email, string caseNumber, CaseStatus status, CancellationToken cancellationToken = default);
    Task NotifyDeliveryChangeOutcomeAsync(string email, string caseNumber, DeliveryChangeOutcome outcome, CancellationToken cancellationToken = default);
}