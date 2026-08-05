namespace SwiftParcel.Application.Common.Interfaces;

public interface ICaseNumberGenerator
{
    Task<string> GenerateNextAsync(CancellationToken cancellationToken = default);
}