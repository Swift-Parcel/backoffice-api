namespace SwiftParcel.Application.Common.Interfaces;

public interface IParcelNumberGenerator
{
    Task<string> GenerateUniqueCodeAsync(CancellationToken cancellationToken = default);
}