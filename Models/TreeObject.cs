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
    public List<TreeChildInfo> ChildInfos { get; set; }
    
    public TreeObject(string dirName) : base(ObjectType.tree)
    {
      this.DirName = dirName;
      this.Content = new List<byte>();
      this.Data = new List<byte>();
      this.Children = new List<SgitObject>();
      this.ChildInfos = new List<TreeChildInfo>();
    }

    protected override string CalculateHash() =>
      HashUtil.CalculateSHA1(Data.ToArray());

    public string ConstructData()
    {
      foreach (var child in Children)
      {
        if (child.Type == ObjectType.blob)
        {
          var blob = (BlobObject)child;
          Content.AddRange(Encoding.UTF8.GetBytes($"100644 {blob.FileName}\x00"));
          Content.AddRange(HashUtil.GetBytes(blob.Hash));
          ChildInfos.Add(new TreeChildInfo("100644", ObjectType.blob, blob.FileName, blob.Hash));
        }
        else
        {
          var tree = (TreeObject)child;
          Content.AddRange(Encoding.UTF8.GetBytes($"40000 {tree.DirName}\x00"));
          var hash = HashUtil.GetBytes(tree.ConstructData());
          Content.AddRange(hash);
          ChildInfos.Add(new TreeChildInfo("040000", ObjectType.tree, tree.DirName, HashUtil.GetString(hash)));
        }
      }
      Size = Buffer.ByteLength(Content.ToArray());
      Data.AddRange(Encoding.UTF8.GetBytes(Header));
      Data.AddRange(Content);
      Hash = CalculateHash();
      return Hash;
    }

    public override string Write()
    {
      foreach (var child in Children)
      {
        if (child.Type == ObjectType.tree)
        {
          var tree = (TreeObject)child;
          tree.Write();
        }
      }
      if (!base.Exists())
      {
        base.Write(Data.ToArray());
      }
      return Hash;
    }
  }
}
