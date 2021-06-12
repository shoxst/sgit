using System;

namespace sgit
{
  public class Program
  {   
    public static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        var b = new BlobObject($"{PathConst.SGIT_REPOSITORY}/file1.txt");
        b.CreateIfNotExists();
        return;
      }
      
      switch (args[0])
      {
        case "init":
          Init.Exec();
          break;

        case "add":
          Add.Exec(args[1]);
          break;
        
        case "commit":
          Commit.Exec(args[2]);
          break;

        default:
          Console.WriteLine("argument error");
          break;
      }
    }
  }
}
