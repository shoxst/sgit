using System;
using System.IO;
using System.Collections.Generic;

namespace sgit
{
  public class TreeObject : SgitObject
  {
    public string DirName { get; set; }
    public List<SgitObject> Children { get; set; }
    
    public TreeObject(string dirName) : base(ObjectType.tree)
    {
      DirName = dirName;
      Children = new List<SgitObject>();
    }

    public string Create()
    {
      string content = "";
      foreach (var child in Children)
      {
        if (child.Type == ObjectType.blob)
        {
          var blob = (BlobObject)child;
          content += $"100644 {Path.GetFileName(blob.FilePath)}\x00{blob.Hash}";
        }
        else
        {
          var tree = (TreeObject)child;
          content += $"40000 {tree.DirName}\x00{tree.Create()}";
        }
      }
      return CreateIfNotExists(content);
    }

    public string CreateIfNotExists(string content)
    {
      this.Content = content;
      if (!base.Exists())
      {
        base.Write();
      }
      return base.CalculateHash();
    }
  }
}
