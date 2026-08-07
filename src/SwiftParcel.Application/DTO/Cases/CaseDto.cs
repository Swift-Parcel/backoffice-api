using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Cases;

public class CaseDto
{
    public int Id { get; set; }
    public string CaseNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CaseType CaseType { get; set; }
    public CaseStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsEscalated { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public DateTime SlaDeadline { get; set; }
    public Channel Channel { get; set; }
    public string? Resolution { get; set; }
    public int? SatisfactionScore { get; set; }

    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    public int? HandlerId { get; set; }
    public string? HandlerName { get; set; }

    public int RegionId { get; set; }
    public string RegionName { get; set; } = string.Empty;

    public List<TagDto> Tags { get; set; } = new();
}

public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}