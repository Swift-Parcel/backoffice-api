using MediatR;
using SwiftParcel.Application.Cases.Events;
using SwiftParcel.Application.Integration.Interfaces;

namespace SwiftParcel.Application.Cases.EventHandlers;

public class CaseStatusChangedEventHandler : INotificationHandler<CaseStatusChangedEvent>
{
    private readonly IWebhookClient _webhookClient;

    public CaseStatusChangedEventHandler(IWebhookClient webhookClient)
    {
        _webhookClient = webhookClient;
    }

    public async Task Handle(CaseStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        await _webhookClient.NotifyCaseStatusChangedAsync(
            notification.CustomerEmail,
            notification.CaseNumber,
            notification.NewStatus,
            cancellationToken
        );
    }
}