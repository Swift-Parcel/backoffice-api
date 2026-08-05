using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Common.Settings;

public class SlaOptions
{
    public const string SectionName = "SlaSettings";
    
    public Dictionary<CaseType, int> DefaultHours { get; set; } = new()
    {
        [CaseType.Lost] = 48,
        [CaseType.Damaged] = 48,
        [CaseType.Delayed] = 48,
        [CaseType.WrongAddress] = 48,
        [CaseType.Billing] = 72,
        [CaseType.DeliveryChange] = 24,
        [CaseType.Other] = 72
    };
}