using MediatR;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Events;

public record CaseStatusChangedEvent(
    string CaseNumber,
    string CustomerEmail,
    CaseStatus NewStatus
) : INotification;