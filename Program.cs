using System;

namespace sgit
{
  public class Program
  {   
    public static void Main(string[] args)
    {
      if (args.Length == 0)
      {
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
          string message = args.Length == 1 ? "" : args[2];
          Commit.Exec(message);
          break;

        default:
          Console.WriteLine("argument error");
          break;
      }
    }
  }
}
