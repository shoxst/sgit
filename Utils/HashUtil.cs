using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace sgit
{
  public static class HashUtil
  {
    public static string CalculateSHA1(string src) =>
      CalculateSHA1(Encoding.UTF8.GetBytes(src));

    public static string CalculateSHA1(byte[] src)
    {
      using (var sha1 = SHA1.Create())
      {
        return GetString(sha1.ComputeHash(src));
      }
    }

    public static byte[] GetBytes(string str) =>
      Enumerable.Range(0, str.Length)
        .Where(x => x % 2 == 0)
        .Select(x => Convert.ToByte(str.Substring(x, 2), 16))
        .ToArray();

    public static string GetString(byte[] ba) =>
      BitConverter.ToString(ba).ToLower().Replace("-","");
  }
}
