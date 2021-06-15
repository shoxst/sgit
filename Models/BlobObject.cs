using System;
using System.IO;

namespace sgit
{
  public class BlobObject : SgitObject
  {
    public string FilePath { get; set; }
    public string FileContent { get; set; }
    public string Data { get; set; }
    private bool canWrite;

    public BlobObject(string filePath) : base(ObjectType.blob)
    {
      this.FilePath = filePath;
      this.FileContent = File.ReadAllText(PathUtil.GetFilePath(FilePath));
      this.Size = FileContent.Length;
      this.Data = Header + FileContent;
      this.Hash = CalculateHash();
      this.canWrite = true;
    }

    public BlobObject(string filePath, string hash) : base(ObjectType.blob)
    {
      this.FilePath = filePath;
      this.Hash = hash;
      this.canWrite = false;
    }

    protected override string CalculateHash() =>
      HashUtil.CalculateSHA1(Data);
    
    public string Write()
    {
      if (!canWrite)
      {
        throw new Exception("This blob object cannot write");
      }
      if (!base.Exists())
      {
        base.Write(Data);
      }
      return Hash;
    }
  }
}
