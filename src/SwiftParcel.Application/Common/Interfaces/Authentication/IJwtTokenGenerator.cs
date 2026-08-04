using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}