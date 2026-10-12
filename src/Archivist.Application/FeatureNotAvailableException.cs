namespace Archivist.Application;

/// <summary>Raised when a contract is available but its production dependency has not been configured.</summary>
public sealed class FeatureNotAvailableException(string feature) : Exception($"{feature} is not configured.")
{
    public string Feature { get; } = feature;
}
