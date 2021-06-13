using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace sgit
{
  public static class Add
  {
    public static void Exec(string path)
    {
      if (!File.Exists(PathUtil.SGIT_INDEX))
      {
        File.Create(PathUtil.SGIT_INDEX).Close();
      }

      var sgitPath = path == PathUtil.SGIT_REPOSITORY ? "" : PathUtil.GetSgitFilePath(path);

      if (File.Exists(path))
      {
        var sgitFilePath = sgitPath;
        var blob = new BlobObject(sgitFilePath);
        var hash = blob.Write();
        Index.Update(sgitFilePath, hash);
      }
      else
      {
        var sub = new Dictionary<string, string>();
        foreach (var filePath in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                                  .Where(filePath => !filePath.StartsWith(PathUtil.SGIT_DOTSGIT)))
        {
          var sgitFilePath = PathUtil.GetSgitFilePath(filePath);
          var blob = new BlobObject(sgitFilePath);
          var hash = blob.Write();
          sub.Add(sgitFilePath, hash);
        }
        Index.Update(sub, sgitPath);
      }
    }
  }
}
