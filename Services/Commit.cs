using System;
using System.Linq;

namespace sgit
{
  public static class Commit
  {
    public static void Exec(string[] args)
    {
      if (args.Length < 3 || args[1] != "-m")
      {
        Console.WriteLine("use 'sgit commit -m \"{message}\"");
        return;
      }
      var message = args[2].Trim('\"');
      
      // Create tree object from index and write file
      var dict = Index.GetDictionary();
      var root = new TreeObject("");
      foreach (var item in dict)
      {
        var dirs = item.Key.Contains("/") ? item.Key.Split('/') : new string[] {item.Key};
        AddChild(item.Value, root, dirs, 0);
      }
      var rootTreeHash = root.WriteTree();

      // Create commit object and write file
      var currentCommit = Reference.GetHeadCommit();
      var parents = currentCommit == null ? null : new string[] {currentCommit};
      var seconds = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
      var author = new UserInfo("*****", "*****", seconds);
      var committer = new UserInfo("*****", "*****", seconds);
      var commit = new CommitObject(rootTreeHash, parents, author, committer, message);
      var newCommit = commit.Write();

      // Update reference
      Reference.SetHeadCommit(newCommit);
    }
    
    private static void AddChild(string blobHash, TreeObject tree, string[] dirs, int depth)
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
      AddChild(blobHash, nextTree, dirs, depth + 1);
    }

    private static bool MatchDirName(SgitObject child, string dirName) =>
      child.Type == ObjectType.tree && ((TreeObject)child).DirName == dirName;
  }
}
