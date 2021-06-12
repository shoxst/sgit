using System;
using System.IO;

namespace sgit
{
  public static class Init
  {
    public static void Exec()
    {
      if (!Directory.Exists(PathUtil.SGIT_DOTSGIT))
      {
        Directory.CreateDirectory(PathUtil.SGIT_OBJECTS);
        Directory.CreateDirectory(PathUtil.SGIT_REFS_HEADS);
        Directory.CreateDirectory(PathUtil.SGIT_REFS_TAGS);
        File.WriteAllText(PathUtil.SGIT_HEAD, "ref: refs/heads/main\n");
      }
      else
      {
        Console.WriteLine("This directory is already sgit repository");
      }
    }
  }
}
