using System;
using System.IO;

namespace sgit
{
  public class BlobObject : SgitObject
  {
    public string FilePath { get; set; }

    public BlobObject(string filePath) : base(ObjectType.blob)
    {
      this.FilePath = filePath;
    }

    public BlobObject(string filePath, string hash) : base(ObjectType.blob)
    {
      this.FilePath = filePath;
      this.Hash = hash;
    }
    
    public string CreateIfNotExists()
    {
      this.Content = File.ReadAllText(FilePath);
      if (!base.Exists())
      {
        base.Write();
      }
      return base.CalculateHash();
    }
  }
}
