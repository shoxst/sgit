using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sgit
{
  public class TagObject : SgitObject
  {
    
    public string TargetHash { get; set; }
    public ObjectType TargetType { get; set; }
    public string Name { get; set; }
    public UserInfo Tagger { get; set; }
    public string Message { get; set; }
    public string Data { get; set; }

    public TagObject(string targetHash, ObjectType targetType, string name, UserInfo tagger, string message)
      : base(ObjectType.tag)
    {
      this.TargetHash = targetHash;
      this.TargetType = targetType;
      this.Name = name;
      this.Tagger = tagger;
      this.Message = message;
      ConstructData();
      this.Hash = CalculateHash();
    }

    private void ConstructData()
    {
      var sb = new StringBuilder();
      sb.Append($"object {TargetHash}\n");
      sb.Append($"type {TargetType}\n");
      sb.Append($"tag {Name}\n");
      sb.Append($"tagger {Tagger.GetString()}\n");
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

    public override void CatFile()
    {
      Console.WriteLine("tag");
      Console.WriteLine(Size);
      Console.WriteLine($"object {TargetHash}");
      Console.WriteLine($"type {TargetType}");
      Console.WriteLine($"tag {Name}");
      Console.WriteLine($"tagger {Tagger.GetString()}");
      Console.WriteLine();
      Console.Write(Message);
    }
  }
}
