using System;
using System.IO;

namespace sgit
{
  public class BlobObject : SgitObject
  {
    public string FileName { get; set; }
    public string FileContent { get; set; }
    public string Data { get; set; }
    private bool canWrite;

    public BlobObject(string filePath) : base(ObjectType.blob)
    {
      this.FileName = Path.GetFileName(filePath);
      this.FileContent = File.ReadAllText(PathUtil.GetFilePath(filePath));
      this.Size = FileContent.Length;
      this.Data = Header + FileContent;
      this.Hash = CalculateHash();
      this.canWrite = true;
    }

    public BlobObject() : base(ObjectType.blob)
    {
      this.canWrite = false;
    }

    protected override string CalculateHash() =>
      HashUtil.CalculateSHA1(Data);
    
    public override string Write()
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

    public override void CatFile()
    {
      Console.WriteLine("blob");
      Console.WriteLine(FileContent.Length);
      Console.WriteLine(FileContent);
    }
  }
}
