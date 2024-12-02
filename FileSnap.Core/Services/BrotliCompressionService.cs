using System.IO.Compression;

namespace FileSnap.Core.Services;

public class BrotliCompressionService : ICompressionService
{
    public byte[] Compress(byte[] data)
    {
        using var msi = new MemoryStream(data);
        using var mso = new MemoryStream();
        using (var bs = new BrotliStream(mso, CompressionMode.Compress))
        {
            msi.CopyTo(bs);
        }
        return mso.ToArray();
    }

    public byte[] Decompress(byte[] data)
    {
        using var msi = new MemoryStream(data);
        using var mso = new MemoryStream();
        using (var bs = new BrotliStream(msi, CompressionMode.Decompress))
        {
            bs.CopyTo(mso);
        }
        return mso.ToArray();
    }
}
