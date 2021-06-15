using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace sgit
{
  public static class Index
  {
    // Key: sgitFilePath, Value: hash
    private static SortedDictionary<string, string> dict;
    
    public static void Update(string sgitFilePath, string hash)
    {
      Read();
      if (dict.ContainsKey(sgitFilePath))
      {
        dict[sgitFilePath] = hash;
      }
      else
      {
        dict.Add(sgitFilePath, hash);
      }
      Write();
    }

    public static SortedDictionary<string, string> GetDictionary()
    {
      Read();
      return dict;
    }

    public static void Update(Dictionary<string, string> target, string dirName)
    {
      Read();
      var itemsToRemove = dict.Where(item => item.Key.StartsWith($"{dirName}")).ToArray();
      foreach (var item in itemsToRemove)
      {
        dict.Remove(item.Key);
      }
      foreach (var pair in target)
      {
        dict.Add(pair.Key, pair.Value);
      }
      Write();
    }

    public static TreeObject GetRootTreeFromIndex()
    {
      Read();
      var root = new TreeObject("");
      foreach (var item in dict)
      {
        var dirs = item.Key.Contains("/") ? item.Key.Split('/') : new string[] {item.Key};
        AddChildObject(item.Value, root, dirs, 0);
      }
      return root;
    }

    private static void AddChildObject(string blobHash, TreeObject tree, string[] dirs, int depth)
    {
      if (depth == dirs.Length - 1)
      {
        var sgitFilePath = string.Join('/', dirs);
        tree.Children.Add(new BlobObject(sgitFilePath, blobHash));
        return;
      }
      if (!tree.Children.Any(child => MatchDirName(child, dirs[depth])))
      {
        tree.Children.Add(new TreeObject(dirs[depth]));
      }
      var nextTree = (TreeObject)tree.Children.Find(child => MatchDirName(child, dirs[depth]));
      AddChildObject(blobHash, nextTree, dirs, depth + 1);
    }

    private static bool MatchDirName(SgitObject child, string dirName) =>
      child.Type == ObjectType.tree && ((TreeObject)child).DirName == dirName;

    private static void Read()
    {
      dict = new SortedDictionary<string, string>();
      using (var sr = new StreamReader(PathUtil.SGIT_INDEX))
      {
        string line;
        while ((line = sr.ReadLine()) != null)
        {
          string[] cols = line.Split(' ');
          dict.Add(cols[3], cols[1]);
        }
      }
    }
    
    private static void Write()
    {
      using (var sw = new StreamWriter(PathUtil.SGIT_INDEX))
      {
        foreach (var item in dict)
        {
          sw.WriteLine($"1060644 {item.Value} 0 {item.Key}");
        }
      }
    }
  }
}
