using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Cases;

public class CreateCustomerCaseRequest
{
    public string Title { get; set; } = string.Empty;
    public required string CustomerEmail { get; set; }
    public List<string> TrackingNumbers { get; set; } = new();
    public CaseType CaseType { get; set; }
    public string Description { get; set; } = string.Empty;
}