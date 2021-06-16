using System;
using System.Linq;
using System.Collections.Generic;

namespace sgit
{
  public static class Log
  {
    public static void Exec(string[] args)
    {
      int num;
      if (args.Length == 3 && args[2] == "-n")
      {
        num = Int32.Parse(args[2]);
      }
      else if (args.Length == 1)
      {
        num = 5;
      }
      else
      {
        Console.WriteLine("use 'sgit log'");
        return;
      }
      
      var headHash = Reference.GetHeadCommit();
      var headCommit = ObjectReader.ReadCommitObject(headHash);

      var child = headCommit;
      var parents = new List<CommitObject>(){child};
      int cnt = 0;
      while (child.Parents.Count !=0 || parents.Count != 0)
      {
        foreach (var hash in child.Parents)
        {
          if (!parents.Any(p => p.Hash == hash))
          {
            parents.Add(ObjectReader.ReadCommitObject(hash));
          }
        }
        var recent = parents.Aggregate((p1, p2) => p1.Author.DateSeconds > p2.Author.DateSeconds ? p1 : p2);
        parents.Remove(recent);
        if (cnt < num)
        {
          recent.PrintLog();
          cnt++;
          child = recent;
        }
        else
        {
          break;
        }
      }
    }
  }
}
