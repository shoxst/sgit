using System;
using System.Text;
using System.Security.Cryptography;

namespace sgit
{
  public static class HashUtil
  {
    public static string CalculateSHA1(string str) {
      using (var sha1 = SHA1.Create())
      {
        var bs = sha1.ComputeHash(new UTF8Encoding().GetBytes(str));
        return BitConverter.ToString(bs).ToLower().Replace("-","");
      }
    }
  }
}