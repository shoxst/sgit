using System.IO;

namespace sgit
{
  public static class SgitConfig
  {
    public static string GetUserName() => 
      GetValue("user", "name");

    public static string GetUserEmail() =>
      GetValue("user", "email");

    private static string GetValue(string key1, string key2)
    {
      using (var sr = new StreamReader(PathUtil.SGIT_CONFIG))
      {
        string line;
        bool matchKey1 = false;
        while ((line = sr.ReadLine()) != null)
        {
          if (matchKey1)
          {
            if (line.StartsWith("[") && line.EndsWith("]"))
            {
              return null;
            }
            var cols = line.Split(" = ");
            if (cols[0].TrimStart() == key2)
            {
              return cols[1];
            }
          }
          else
          {
            if (line.Equals($"[{key1}]"))
            {
              matchKey1 = true;
            }
          }
        }
        return null;
      }
    }
  }
}
