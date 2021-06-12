using System;
using System.IO;

namespace sgit
{
  public class BlobObject : SgitObject
  {
    public string FilePath { get; set; }
    public string FileContent { get; set; }

    public BlobObject(string filePath) : base(ObjectType.blob)
    {
      this.FilePath = filePath;
      this.FileContent = File.ReadAllText(PathUtil.GetFilePath(FilePath));
      this.Size = FileContent.Length;
    }

    public BlobObject(string filePath, string hash) : base(ObjectType.blob)
    {
      this.FilePath = filePath;
      this.Hash = hash;
    }

    protected override string CalculateHash()
    {
      return HashUtil.CalculateSHA1(Header + FileContent);
    }
    
    public string CreateIfNotExists()
    {
      if (!base.Exists())
      {
        base.Write(Header + FileContent);
      }
      return CalculateHash();
    }
  }
}
