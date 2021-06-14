using System;
using System.Text;
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
      this.DirName = dirName;
      this.Content = new List<byte>();
      this.Data = new List<byte>();
      this.Children = new List<SgitObject>();
    }

    protected override string CalculateHash() =>
      HashUtil.CalculateSHA1(Data.ToArray());

    public string WriteTree()
    {
      foreach (var child in Children)
      {
        if (child.Type == ObjectType.blob)
        {
          var blob = (BlobObject)child;
          Content.AddRange(Encoding.UTF8.GetBytes($"100644 {Path.GetFileName(blob.FilePath)}\x00"));
          Content.AddRange(HashUtil.GetBytes(blob.Hash));
        }
        else
        {
          var tree = (TreeObject)child;
          Content.AddRange(Encoding.UTF8.GetBytes($"40000 {tree.DirName}\x00"));
          Content.AddRange(HashUtil.GetBytes(tree.WriteTree()));
        }
      }
      Size = Buffer.ByteLength(Content.ToArray());
      Data.AddRange(Encoding.UTF8.GetBytes(Header));
      Data.AddRange(Content);
      Hash = CalculateHash();
      return this.Write();
    }

    private string Write()
    {
      if (!base.Exists())
      {
        base.Write(Data.ToArray());
      }
      return Hash;
    }
  }
}
