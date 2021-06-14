using System;
using System.Collections.Generic;

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
      var message = args[2].Trim('\"')+'\n';
      
      // Create tree object and write file
      var root = Index.GetRootTreeFromIndex();
      var rootTreeHash = root.WriteTree();

      // Create commit object and write file
      var currentCommit = Reference.GetHeadCommit();
      var parents = currentCommit == null ? new List<string>() : new List<string>{currentCommit};
      var seconds = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
      var timezone = TimeZoneInfo.Local.DisplayName.Substring(4,6).Replace(":","");
      var author = new UserInfo("*****", "*****", seconds, timezone);
      var committer = new UserInfo("*****", "*****", seconds, timezone);
      var commit = new CommitObject(rootTreeHash, parents, author, committer, message);
      var newCommit = commit.Write();

      // Update reference
      Reference.SetHeadCommit(newCommit);
    }
  }
}
