using System;
using System.Linq;

namespace sgit
{
  public static class Branch
  {
    public static void Exec(string[] args)
    {
      // 'sgit branch'
      if (args.Length == 1)
      {
        ListBranchNames();
        return;
      }
      // 'sgit branch {name}'
      if (args.Length == 2)
      {
        CreateNewBranch(args[1]);
        return;
      }
      // 'sgit branch -d {name}'
      if (args.Length == 3 && args[1] == "-d")
      {
        DeleteBranch(args[2]);
        return;
      }
    }

    private static void ListBranchNames()
    {
      foreach (var name in Reference.GetAllBranchNames())
      {
        if (name == Reference.GetHead())
        {
          Console.WriteLine($"* {name}");
        }
        else
        {
          Console.WriteLine($"  {name}");
        }
      }
    }

    private static void CreateNewBranch(string name)
    {
      if (Reference.GetAllBranchNames().Any(b => b == name))
      {
        Console.WriteLine($"A branch named '{name}' already exists");
        return;
      }
      var head = Reference.GetHead();
      var commit = Reference.GetHeadCommit();
      if (commit == null)
      {
        Console.WriteLine($"Not a valid reference name: '{head}'");
        return;
      }
      Reference.SetCommit(name, commit);
    }

    private static void DeleteBranch(string name)
    {
      if (!Reference.GetAllBranchNames().Any(b => b == name))
      {
        Console.WriteLine($"branch '{name}' not found");
        return;
      }
      if (name == Reference.GetHead())
      {
        Console.WriteLine($"Cannnot delete branch '{name}' because checked out");
        return;
      }
      Reference.DeleteBranch(name);
      Console.WriteLine($"Deleted branch {name}");
    }
  }
}
