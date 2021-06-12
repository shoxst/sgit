using System;
using System.IO;

namespace sgit
{
  public static class Init
  {
    public static void Exec()
    {
      if (!Directory.Exists(PathConst.SGIT_DOTSGIT))
      {
        Directory.CreateDirectory(PathConst.SGIT_OBJECTS);
        Directory.CreateDirectory(PathConst.SGIT_REFS_HEADS);
        Directory.CreateDirectory(PathConst.SGIT_REFS_TAGS);
        File.WriteAllText(PathConst.SGIT_HEAD, "ref: refs/heads/main\n");
      }
      else
      {
        Console.WriteLine("This directory is already sgit repository");
      }
    }
  }
}
