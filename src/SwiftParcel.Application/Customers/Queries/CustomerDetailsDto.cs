using SwiftParcel.Application.DTO;

namespace SwiftParcel.Application.Customers.Queries;

public record CustomerDetailsDto(
    string FullName,
    string Email,
    string Phone,
    DateTimeOffset RegisteredDate,
    bool Vip,
    string? Notes,
    AddressDto? Address
);