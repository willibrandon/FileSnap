namespace FileSnap.Core.Services;

public interface ICompressionService
{
    byte[] Compress(byte[] data);
    byte[] Decompress(byte[] data);
}
