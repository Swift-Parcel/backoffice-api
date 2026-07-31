using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Integration.Interfaces;

public interface IWebhookClient
{
    Task NotifyParcelStatusChangedAsync(string trackingNumber, ParcelStatus status, CancellationToken cancellationToken = default);
    Task NotifyCaseStatusChangedAsync(string caseNumber, CaseStatus status, CancellationToken cancellationToken = default);
}