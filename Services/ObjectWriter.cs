using System.IO;

namespace sgit
{
  public static class ObjectWriter
  {
    public static void Write(string hash, byte[] data)
    {
      var filePath = PathUtil.GetObjectFilePath(hash);
      var dirName = Path.GetDirectoryName(filePath);
      if (!Directory.Exists(dirName))
      {
        Directory.CreateDirectory(dirName);
      }
      var cData = CompressUtil.Compress(data);
      File.WriteAllBytes(filePath, cData);
    }
  }
}
