using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sgit
{
  public class CommitObject : SgitObject
  {
    public string TreeHash { get; set; }
    public List<string> Parents { get; set; }
    public UserInfo Author { get; set; }
    public UserInfo Committer { get; set; }
    public string Message { get; set; }
    public string Data { get; set; }
    
    public CommitObject(string treeHash, List<string> parents, UserInfo author, UserInfo commiter, string message)
      : base(ObjectType.commit)
    {
      this.TreeHash = treeHash;
      this.Parents = parents;
      this.Author = author;
      this.Committer = commiter;
      this.Message = message;
      ConstructData();
      this.Hash = CalculateHash();
    }
    
    private void ConstructData()
    {
      var sb = new StringBuilder();
      sb.Append($"tree {TreeHash}\n");
      if (Parents.Count != 0)
      {
        foreach (var parent in Parents)
        {
          sb.Append($"parent {parent}\n");
        }
      }
      sb.Append($"author {Author.Name} <{Author.Email}> {Author.DateSeconds} {Author.DateTimezone}\n");
      sb.Append($"committer {Committer.Name} <{Committer.Email}> {Committer.DateSeconds} {Committer.DateTimezone}\n");
      sb.Append("\n");
      sb.Append($"{Message}");
      var content = sb.ToString();
      this.Size = content.Length;
      this.Data = Header + content;
    }
    
    protected override string CalculateHash() =>
      HashUtil.CalculateSHA1(Data);

    public override string Write()
    {
      base.Write(Data);
      return Hash;
    }

    public void PrintLog()
    {
      var branchNames = Reference.GetAllBranchNames();
      var branchCommits = branchNames.Select(branchName => Reference.GetCommit(branchName)).ToList();
      var head = Reference.GetHead();
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.Write($"commit {Hash} ");
      Console.ResetColor();
      if (branchCommits.Contains(Hash))
      {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("(");
        if (Hash == Reference.GetCommit(head))
        {
          Console.ForegroundColor = ConsoleColor.Cyan;
          Console.Write("HEAD -> ");
          Console.ForegroundColor = ConsoleColor.Green;
          Console.Write($"{head}");
          Console.ForegroundColor = ConsoleColor.Yellow;
          Console.Write(", ");
        }
        foreach (var branchName in branchNames)
        {
          if (branchName == head) continue;
          if (Hash == Reference.GetCommit(branchName))
          {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{branchName}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(", ");
          }
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\b\b)");
        Console.ResetColor();
      }
      var tz = Author.DateTimezone;
      var timeSpan = new TimeSpan(Int32.Parse(tz.Substring(0,3)),Int32.Parse(tz.Substring(3,2)),0);
      var utcTime = DateTimeOffset.FromUnixTimeSeconds(Author.DateSeconds);
      var authorTime = utcTime.ToOffset(timeSpan);
      Console.Write("\n");
      Console.WriteLine($"Author: {Author.Name} <{Author.Email}>");
      Console.WriteLine($"Date:   {authorTime.ToString("yyyy/MM/dd HH:mm:ss")} {tz}");
      Console.WriteLine();
      var lines = Message.Split('\n');
      lines.ToList().ForEach(line => Console.WriteLine($"\t{line}"));
    }
  }
}
