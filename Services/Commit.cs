using System;
using System.IO;

namespace sgit
{
  public static class Commit
  {
    public static void Exec(string message)
    {
      var root = new TreeObject("root");
      var a = new TreeObject("a");
      var file2 = new BlobObject("a/file2.txt", "ded");
      var file1 = new BlobObject("file1.txt", "a12");
      a.Children.Add(file2);
      root.Children.Add(a);
      root.Children.Add(file1);
      var rootTreeHash = root.Create();
    }
  }
}
