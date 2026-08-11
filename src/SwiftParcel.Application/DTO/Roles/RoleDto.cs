namespace SwiftParcel.Application.DTO.Roles;

public record RoleDto(
    int Id, 
    string Name, 
    string Description, 
    bool CanAccessAllRegions, 
    bool IsActive);