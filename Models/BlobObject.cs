using System;
using System.IO;

namespace sgit
{
  public class BlobObject : SgitObject
  {
    public BlobObject() : base(ObjectType.blob)
    {
    }
    
    public string CreateIfNotExists(string filePath)
    {
      this.Content = File.ReadAllText(filePath);
      if (!base.Exists())
      {
        base.Write();
      }
      return base.CalculateHash();
    }
  }
}
