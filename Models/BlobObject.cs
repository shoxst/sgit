using System;
using System.IO;

namespace sgit
{
  public class BlobObject : SgitObject
  {
    public string FilePath { get; set; }
    public string FileContent { get; set; }
    public string Data { get; set; }
    public string Hash { get; set; }

    public BlobObject(string filePath) : base(ObjectType.blob)
    {
      this.FilePath = filePath;
      this.FileContent = File.ReadAllText(PathUtil.GetFilePath(FilePath));
      this.Size = FileContent.Length;
      this.Data = Header + FileContent;
    }

    public BlobObject(string filePath, string hash) : base(ObjectType.blob)
    {
      this.FilePath = filePath;
      this.Hash = hash;
    }

    protected override string CalculateHash() =>
      HashUtil.CalculateSHA1(Data);
    
    public string Write()
    {
      if (!base.Exists())
      {
        base.Write(Data);
      }
      return CalculateHash();
    }
  }
}
