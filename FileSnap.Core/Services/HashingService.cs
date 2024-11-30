using System.Security.Cryptography;

namespace FileSnap.Core.Services;

/// <summary>
/// Provides methods for computing file hashes.
/// </summary>
public class HashingService : IHashingService
{
    /// <inheritdoc />
    public async Task<string> ComputeHashAsync(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await sha256.ComputeHashAsync(stream);
        return Convert.ToBase64String(hash);
    }
}
