using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace sgit
{
  public class Program
  {   
    public static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        var b = new BlobObject();
        b.CreateIfNotExists($"{PathConst.SGIT_REPOSITORY}/file1.txt");
        return;
      }
      
      switch (args[0])
      {
        case "init":
          sgitInit();
          break;

        case "add":
          sgitAdd(args[1]);
          break;
          
        default:
          Console.WriteLine("argument error");
          break;
      }
      
    }

    public static void sgitInit()
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

    public static void sgitAdd(string path)
    {
      if (!File.Exists(PathConst.SGIT_INDEX))
      {
        File.Create(PathConst.SGIT_INDEX);
      }

      if (File.Exists(path))
      {
        var filePath = path;
        var b = new BlobObject();
        var hash = b.CreateIfNotExists(filePath);
        Index.Update(filePath, hash);
      }
      else
      {
        var sub = new Dictionary<string, string>();
        foreach (var filePath in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                                  .Where(filePath => !filePath.StartsWith(PathConst.SGIT_DOTSGIT)))
        {
          var b = new BlobObject();
          var hash = b.CreateIfNotExists(filePath);
          sub.Add(filePath, hash);
        }
        Index.Update(sub, path);
      }
    }

    public static void ShowBytes(byte[] bs)
    {
      Array.ForEach(bs, b => Console.Write($"0x{b:x2} "));
      Console.WriteLine("");
    }
  }
}
