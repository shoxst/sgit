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
      
      // Create tree object and write file
      var root = Index.GetRootTreeFromIndex();
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
  }
}
