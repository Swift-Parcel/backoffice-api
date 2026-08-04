using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Common.Interfaces.Webhooks;

public interface ICustomerPortalNotificationService
{
    Task NotifyParcelStatusChangedAsync(string trackingNumber, ParcelStatus newStatus, CancellationToken cancellationToken = default);
    Task NotifyCaseStatusChangedAsync(string caseNumber, CaseStatus newStatus, string? resolution, CancellationToken cancellationToken = default);
    Task NotifyDeliveryChangeResolvedAsync(string caseNumber, bool isApproved, CancellationToken cancellationToken = default);
}