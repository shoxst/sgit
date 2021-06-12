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
      if (!File.Exists(PathConst.SGIT_INDEX))
      {
        File.Create(PathConst.SGIT_INDEX);
      }

      if (File.Exists(path))
      {
        var filePath = path;
        var blob = new BlobObject(filePath);
        var hash = blob.CreateIfNotExists();
        Index.Update(filePath, hash);
      }
      else
      {
        var sub = new Dictionary<string, string>();
        foreach (var filePath in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                                  .Where(filePath => !filePath.StartsWith(PathConst.SGIT_DOTSGIT)))
        {
          var blob = new BlobObject(filePath);
          var hash = blob.CreateIfNotExists();
          sub.Add(filePath, hash);
        }
        Index.Update(sub, path);
      }
    }
  }
}
