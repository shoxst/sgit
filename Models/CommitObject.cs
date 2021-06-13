using System;
using System.Text;

namespace sgit
{
  public class CommitObject : SgitObject
  {
    public string TreeHash { get; set; }
    public string[] Parents { get; set; }
    public UserInfo Author { get; set; }
    public UserInfo Committer { get; set; }
    public string Message { get; set; }
    public string Data { get; set; }
    
    public CommitObject(string treeHash, string[] parents, UserInfo author, UserInfo commiter, string message)
      : base(ObjectType.commit)
    {
      TreeHash = treeHash;
      Parents = parents;
      Author = author;
      Committer = commiter;
      Message = message;
      ConstructData();
    }

    private void ConstructData()
    {
      var sb = new StringBuilder();
      sb.Append($"tree {TreeHash}\n");
      if (Parents != null)
      {
        foreach (var parent in Parents)
        {
          sb.Append($"parent {parent}\n");
        }
      }
      sb.Append($"author {Author.Name} <{Author.Email}> {Author.DateSeconds} {Author.DateTimezone}\n");
      sb.Append($"committer {Committer.Name} <{Committer.Email}> {Committer.DateSeconds} {Committer.DateTimezone}\n");
      sb.Append("\n");
      sb.Append($"{Message}\n");
      var content = sb.ToString();
      Size = content.Length;
      Data = Header + content;
    }
    
    protected override string CalculateHash() =>
      HashUtil.CalculateSHA1(Data);

    public string Write()
    {
      base.Write(Data);
      return CalculateHash();
    }
  }
}
