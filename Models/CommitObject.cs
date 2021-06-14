using System;
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

    public string Write()
    {
      base.Write(Data);
      return Hash;
    }
  }
}
