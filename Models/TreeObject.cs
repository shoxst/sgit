using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace sgit
{
  public class TreeObject : SgitObject
  {
    public string DirName { get; set; }
    public List<byte> Content { get; set; }
    public List<byte> Data { get; set; }
    public List<SgitObject> Children { get; set; }
    
    public TreeObject(string dirName) : base(ObjectType.tree)
    {
      DirName = dirName;
      Content = new List<byte>();
      Data = new List<byte>();
      Children = new List<SgitObject>();
    }

    protected override string CalculateHash()
    {
      return HashUtil.CalculateSHA1(Data.ToArray());
    }

    public string Create()
    {
      foreach (var child in Children)
      {
        if (child.Type == ObjectType.blob)
        {
          var blob = (BlobObject)child;
          Content.AddRange(new UTF8Encoding().GetBytes($"100644 {Path.GetFileName(blob.FilePath)}\x00"));
          Content.AddRange(HashUtil.GetBytes(blob.Hash));
        }
        else
        {
          var tree = (TreeObject)child;
          Content.AddRange(new UTF8Encoding().GetBytes($"40000 {tree.DirName}\x00"));
          Content.AddRange(HashUtil.GetBytes(tree.Create()));
        }
      }
      Size = Content.ToArray().Length;
      Data.AddRange(new UTF8Encoding().GetBytes(Header));
      Data.AddRange(Content.ToArray());
      return CreateIfNotExists();
    }

    public string CreateIfNotExists()
    {
      if (!base.Exists())
      {
        base.Write(Data.ToArray());
      }
      return CalculateHash();
    }
  }
}
