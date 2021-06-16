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
      
      // sgit command
      switch (args[0])
      {
        case "init":
          Init.Exec();
          break;

        case "add":
          Add.Exec(args);
          break;
        
        case "commit":
          Commit.Exec(args);
          break;

        case "branch":
          Branch.Exec(args);
          break;
        
        case "status":
          Status.Exec();
          break;
        
        case "log":
          Log.Exec(args);
          break;

        default:
          Console.WriteLine("argument error");
          break;
      }
    }
  }
}
