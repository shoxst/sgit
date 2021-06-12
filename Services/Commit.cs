using System;
using System.Linq;
using System.IO;

namespace sgit
{
  public static class Commit
  {
    public static void Exec(string message)
    {
      var indexes = Index.GetIndexes();
      var root = new TreeObject("root");
      foreach (var index in indexes)
      {
        string[] dirs = index.Key.Contains("/") ? index.Key.Split('/') : new string[] {index.Key};
        AddChild(index.Value, root, dirs, 0);
      }
      var rootTreeHash = root.Create();
      
    }
    private static void AddChild(string blobHash, TreeObject tree, string[] dirs, int i)
    {
      if (i == dirs.Length - 1)
      {
        var sgitFilePath = string.Join('/', dirs);
        tree.Children.Add(new BlobObject(sgitFilePath, blobHash));
        return;
      }
      if (!tree.Children.Any(child => child.Type == ObjectType.tree && ((TreeObject)child).DirName == dirs[i]))
      {
        tree.Children.Add(new TreeObject(dirs[i]));
      }
      var nextTree = (TreeObject)tree.Children.Find(child => child.Type == ObjectType.tree && ((TreeObject)child).DirName == dirs[i]);
      AddChild(blobHash, nextTree, dirs, i+1);
    }
  }
}
