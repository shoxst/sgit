using System;
using System.Text;
using System.IO;

namespace sgit
{
  public enum ObjectType { commit, tree, blob }
  
  public abstract class SgitObject
  {
    public ObjectType Type { get; set; }
    public int Size { get; set; }
    public string Header { get { return Type + " " + Size + "\x00"; } }
    public string Hash { get; set; }

    public SgitObject(ObjectType type)
    {
      this.Type = type;
    }

    protected abstract string CalculateHash();

    public bool Exists()
    {
      return File.Exists(GetTargetFilePath());
    }

    public void Write(string data)
    {
      Write(new UTF8Encoding().GetBytes(data));
    }

    public void Write(byte[] data)
    {
      var cData = CompressUtil.Compress(data);
      var filePath = GetTargetFilePath();
      var dirName = Path.GetDirectoryName(filePath);
      if (!Directory.Exists(dirName))
      {
        Directory.CreateDirectory(dirName);
      }
      File.WriteAllBytes(filePath, cData);
    }

    private string GetTargetFilePath() {
      var hash = CalculateHash();
      var dirName = hash.Substring(0,2);
      var fileName = hash.Substring(2);
      return $"{PathUtil.SGIT_OBJECTS}/{dirName}/{fileName}";
    }
  }
}
