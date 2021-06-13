using System;
using System.Linq;

namespace sgit
{
  public static class Commit
  {
    public static void Exec(string message)
    {
      var indexes = Index.GetIndexes();
      var root = new TreeObject("");
      foreach (var index in indexes)
      {
        var dirs = index.Key.Contains("/") ? index.Key.Split('/') : new string[] {index.Key};
        AddChild(index.Value, root, dirs, 0);
      }

      var rootTreeHash = root.WriteTree();
      var currentCommit = Reference.GetHeadCommit();
      var parents = currentCommit == null ? null : new string[] { currentCommit };
      var author = new UserInfo("*****", "*****", 1623566933, "+0900");
      var committer = new UserInfo("*****", "*****", 1623566933, "+0900");
      var commit = new CommitObject(rootTreeHash, parents, author, committer, message);
      var newCommit = commit.Write();

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
