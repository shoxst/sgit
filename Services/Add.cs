using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace sgit
{
  public static class Add
  {
    public static void Exec(string[] args)
    {
      if (args.Length == 1)
      {
        Console.WriteLine("use 'sgit add {file}|{directory}'");
        return;
      }
      var path = args[1];
      var sgitPath = path == PathUtil.SGIT_REPOSITORY ? "" : PathUtil.GetSgitFilePath(path);
      
      // create index file when first commit
      if (!File.Exists(PathUtil.SGIT_INDEX))
      {
        File.Create(PathUtil.SGIT_INDEX).Close();
      }

      if (File.Exists(path))
      {
        // when argument is file
        var sgitFilePath = sgitPath;
        var blob = new BlobObject(sgitFilePath);
        var hash = blob.Write();
        Index.Update(sgitFilePath, hash);
      }
      else
      {
        // when argument is directory
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
