namespace SwiftParcel.Application.DTO.Customers;

public record CreateCustomerRequest(
     string Name,
     string Email,
     string Phone,
     AddressDto Address
);