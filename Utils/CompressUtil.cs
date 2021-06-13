using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace sgit
{
  public static class CompressUtil
  {
    public static byte[] Compress(byte[] src)
    {
      byte[] csrc;
      using (var ms = new MemoryStream())
      {
        using (var ds = new DeflateStream(ms, CompressionMode.Compress, true))
        {
          ds.Write(src, 0, src.Length);
        }
        csrc = ms.ToArray();
      }

      using (var ms = new MemoryStream())
      {
        var zlibHeader = new byte[] { 0x78, 0x01 };
        ms.Write(zlibHeader, 0, zlibHeader.Length);
        ms.Write(csrc, 0, csrc.Length);

        var adler32 = new Adler32();
        var checksum = adler32.ComputeHash(src);
        ms.Write(checksum, 0, checksum.Length);

        return ms.ToArray();
      }
    }
  }

  public class Adler32 : HashAlgorithm
  {
    private const uint BASE = 65521; /* largest prime smaller than 65536 */
    private uint s1;
    private uint s2;

    public Adler32()
    {
      base.HashSizeValue = 32;
      Initialize();
    }
    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
      while (--cbSize >= 0) {
        s1 = (s1 + array[ibStart++]) % BASE;
        s2 = (s2 + s1) % BASE;
      }         
    }
    protected override byte[] HashFinal()
    {
      HashValue = new byte[] {
        (byte)((s2 >> 8) & 0xFF), 
        (byte)( s2       & 0xFF), 
        (byte)((s1 >> 8) & 0xFF), 
        (byte)( s1       & 0xFF)
      };
      return HashValue;
    }
    public override void Initialize()
    {
      s1 = 1;
      s2 = 0;
    }
  }
}
