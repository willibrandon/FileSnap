namespace FileSnap.Core.Services;

/// <summary>
/// Defines methods for computing file hashes.
/// </summary>
public interface IHashingService
{
    /// <summary>
    /// Computes the hash of the file at the specified path.
    /// </summary>
    /// <param name="filePath">The path of the file to compute the hash for.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the computed hash.</returns>
    Task<string> ComputeHashAsync(string filePath);
}

