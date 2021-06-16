using System;

namespace sgit
{
  public static class CatFile
  {
    public static void Exec(string[] args)
    {
      if (args.Length != 2)
      {
        Console.WriteLine("use 'sgit cat-file {hash}'");
      }
      var hash = args[1];

      var fileContent = ObjectReader.ReadBlobObject(hash);
      if (fileContent != null)
      {
        Console.WriteLine("blob");
        Console.WriteLine(fileContent.Length);
        Console.WriteLine(fileContent);
        return;
      }
      
      var tree = ObjectReader.ReadTreeObject(hash);
      if (tree != null)
      {
        Console.WriteLine("tree");
        Console.WriteLine(tree.Size);
        foreach (var info in tree.ChildInfos)
        {
          Console.WriteLine($"{info.Mode} {info.Type} {info.Hash} {info.Name}");
        }
        return;
      }

      var commit = ObjectReader.ReadCommitObject(hash);
      if (commit != null)
      {
        Console.WriteLine("commit");
        Console.WriteLine(commit.Size);
        Console.WriteLine($"tree {commit.TreeHash}");
        foreach (var parent in commit.Parents)
        {
          Console.WriteLine($"parent {parent}");   
        }
        var author = commit.Author;
        Console.WriteLine($"author {author.Name} <{author.Email}> {author.DateSeconds} {author.DateTimezone}");
        var committer = commit.Author;
        Console.WriteLine($"author {committer.Name} <{committer.Email}> {committer.DateSeconds} {committer.DateTimezone}");
        Console.WriteLine();
        Console.Write(commit.Message);
        return;
      }
    }
  }
}
