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

    public abstract string Write();

    public bool Exists() =>
      File.Exists(PathUtil.GetObjectFilePath(Hash));

    public void Write(string data) =>
      Write(Encoding.UTF8.GetBytes(data));

    public void Write(byte[] data) =>
      ObjectWriter.Write(Hash, data);
  }
}
