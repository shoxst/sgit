using System;
using System.IO;

namespace sgit
{
  public enum ObjectType { commit, tree, blob }
  public abstract class SgitObject
  {
    public ObjectType Type { get; set; }
    public string Content { get; set; }
    public int Size { get { return Content.Length; } }
    public string Data { get {return Type + " " + Size + "\x00" + Content; } }
    public string Hash { get; set; }

    public SgitObject(ObjectType type)
    {
      this.Type = type;
    }
    public string CalculateHash()
    {
      Hash = HashUtil.CalculateSHA1(Data);
      return Hash;
    }
    public bool Exists()
    {
      return File.Exists(GetTargetFilePath());
    }
    public void Write()
    {
      var cData = CompressUtil.Compress(Data);
      var filePath = GetTargetFilePath();
      Directory.CreateDirectory(Path.GetDirectoryName(filePath));
      File.WriteAllBytes(filePath, cData);
    }

    private string GetTargetFilePath() {
      var hash = CalculateHash();
      var dirName = hash.Substring(0,2);
      var fileName = hash.Substring(2);
      return $"{PathConst.SGIT_OBJECTS}/{dirName}/{fileName}";
    }
  }
}
