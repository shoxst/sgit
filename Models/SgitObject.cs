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

    public SgitObject(ObjectType type)
    {
      this.Type = type;
    }
    public string CalculateHash()
    {
      return HashUtil.CalculateSHA1(Data);
    }
    public bool Exists()
    {
      return Directory.Exists(GetFilePath());
    }
    public void Write()
    {
      var cData = CompressUtil.Compress(Data);
      var filePath = GetFilePath();
      Directory.CreateDirectory(Path.GetDirectoryName(filePath));
      File.WriteAllBytes(filePath, cData);
    }

    private string GetFilePath() {
      var hash = CalculateHash();
      var dirName = hash.Substring(0,2);
      var fileName = hash.Substring(2);
      return $".sgit/objects/{dirName}/{fileName}";
    }
  }
}
