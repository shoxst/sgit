using System;
using System.IO;

namespace sgit
{
  class Program
  {
    static void Main(string[] args)
    {
      var b = new BlobObject();
      b.Content = "hello\n";
      b.Write();
    }

    public static void ShowBytes(byte[] bs)
    {
      Array.ForEach(bs, b => Console.Write($"0x{b:x2} "));
      Console.WriteLine("");
    }
  }
}
