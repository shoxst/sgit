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

      // working tree status check
      bool headIdxDiff = StatusChecker.CompareHeadAndIndex();
      bool idxWkdDiff = StatusChecker.CompareIndexAndWorkingDirectory();
      // when no commit object
      if (!headIdxDiff)
      {
        if (!idxWkdDiff)
        {
          StatusChecker.PrintWorkingTreeClean();
        }
        else
        {
          StatusChecker.PrintIndexAndWorkingDirectoryDiff();
        }
        return;
      }

      // Create tree object and write file
      var root = Index.GetRootTreeFromIndex();
      root.ConstructData();
      var rootTreeHash = root.Write();

      // Create commit object and write file
      var currentCommit = Reference.GetHeadCommit();
      var parents = currentCommit == null ? new List<string>() : new List<string>{currentCommit};
      var seconds = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
      var timezone = TimeZoneInfo.Local.DisplayName.Substring(4,6).Replace(":","");
      var author = new UserInfo(SgitConfig.GetUserName(), SgitConfig.GetUserEmail(), seconds, timezone);
      var committer = new UserInfo(SgitConfig.GetUserName(), SgitConfig.GetUserEmail(), seconds, timezone);
      var commit = new CommitObject(rootTreeHash, parents, author, committer, message);
      var newCommit = commit.Write();

      // Update reference
      Reference.SetHeadCommit(newCommit);

      // print message
      var head = Reference.GetHead();
      Console.WriteLine($"[{head} {newCommit.Substring(0,7)}] {message.Substring(0,message.IndexOf('\n'))}");
      int changed = StatusChecker.HeadIdxModified.Count;
      if (changed != 0)
      {
        var s = changed == 1 ? "" : "s";
        Console.WriteLine($" {changed} file{s} changed");
      }
      foreach (var file in StatusChecker.HeadIdxNew)
      {
        Console.WriteLine($" create mode 100644 {file}");
      }
      foreach (var file in StatusChecker.HeadIdxDeleted)
      {
        Console.WriteLine($" delete mode 100644 {file}");
      }
    }
  }
}
