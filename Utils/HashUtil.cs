using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace sgit
{
  public static class HashUtil
  {
    public static string CalculateSHA1(string src) =>
      CalculateSHA1(new UTF8Encoding().GetBytes(src));

    public static string CalculateSHA1(byte[] src)
    {
      using (var sha1 = SHA1.Create())
      {
        var bs = sha1.ComputeHash(src);
        return BitConverter.ToString(bs).ToLower().Replace("-","");
      }
    }

    public static byte[] GetBytes(string hash)
    {
      return Enumerable.Range(0, hash.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hash.Substring(x, 2), 16))
                .ToArray();
    }
  }
}
