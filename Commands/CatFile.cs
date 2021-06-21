using System;

namespace sgit
{
  public static class CatFile
  {
    public static void Exec(string[] args)
    {
      if (args.Length != 2)
      {
        Console.WriteLine("use 'sgit cat-file {hash}'");
      }
      var hash = args[1];
      var obj = ObjectReader.ReadObject(hash);
      if (obj != null)
      {
        obj.CatFile();
      }
      else
      {
        Console.WriteLine($"Not a valid object name {hash}");
      }
    }
  }
}
