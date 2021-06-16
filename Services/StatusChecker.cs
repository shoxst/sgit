using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace sgit
{
  public static class StatusChecker
  {
    public static List<string> HeadIdxModified { get; set; }  = new List<string>();
    public static List<string> HeadIdxDeleted { get; set; } = new List<string>();
    public static List<string> HeadIdxNew { get; set; } = new List<string>();
    public static List<string> IdxWkdModified { get; set; } = new List<string>();
    public static List<string> IdxWkdDeleted { get; set; } = new List<string>();
    public static List<string> IdxWkdUntracked { get; set; } = new List<string>();

    public static bool CompareHeadAndIndex()
    {      
      if (!File.Exists(PathUtil.SGIT_INDEX))
      {
        return false;
      }
      var index = Index.GetDictionary();

      var commit = Reference.GetHeadCommit();
      if (commit == null)
      {
        // when no commit yet
        foreach (var sgitFilePath in index.Keys)
        {
          HeadIdxNew.Add(sgitFilePath);
        }
        return HeadIdxNew.Count != 0;
      }
      var head = ObjectReader.GetBlobHashFromCommit(commit);
      var copy = new Dictionary<string, string>(head);

      foreach (var item in index)
      {
        if (!head.TryGetValue(item.Key, out var hash))
        {
          // Exists in index, not in head
          HeadIdxNew.Add(item.Key);
          continue;
        }
        if (item.Value != hash)
        {
          // File content is different between index and head
          HeadIdxModified.Add(item.Key);
        }
        copy.Remove(item.Key);
      }
      foreach (var sgitFilePath in copy.Keys)
      {
        // Exists in head, not in index
        HeadIdxDeleted.Add(sgitFilePath);
      }
      return HeadIdxModified.Count != 0 || HeadIdxDeleted.Count != 0 || HeadIdxNew.Count != 0;
    }

    public static bool CompareIndexAndWorkingDirectory()
    {
      var sgitFilePaths = PathUtil.GetAllFilesUnderPath(PathUtil.SGIT_REPOSITORY, true)
                        .Select(filePath => PathUtil.GetSgitFilePath(filePath));

      if (!File.Exists(PathUtil.SGIT_INDEX))
      {
        IdxWkdUntracked.AddRange(sgitFilePaths);
        return IdxWkdUntracked.Count != 0;
      }

      var index = Index.GetDictionary();
      var copy = new Dictionary<string, string>(index);

      foreach (var sgitFilePath in sgitFilePaths)
      {
        if (!index.TryGetValue(sgitFilePath, out var hash))
        {
          // Exists in working directory, not in index
          IdxWkdUntracked.Add(sgitFilePath);
          continue;
        }
        var blob = new BlobObject(sgitFilePath);
        if (blob.Hash != hash)
        {
          // File content is different between working directory and index
          IdxWkdModified.Add(sgitFilePath);
        }
        copy.Remove(sgitFilePath);
      }
      foreach (var sgitFilePath in copy.Keys)
      {
        // Exists in index, not in working directory
        IdxWkdDeleted.Add(sgitFilePath);
      }
      return IdxWkdModified.Count != 0 || IdxWkdDeleted.Count != 0 || IdxWkdUntracked.Count != 0;
    }

    public static void PrintWorkingTreeClean()
    {
      Console.WriteLine($"On branch {Reference.GetHead()}");
      Console.WriteLine("nothing to commit, working tree clean");
    }

    public static void PrintHeadAndIndexDiff()
    {
      Console.WriteLine("Changes to be commited");
      Console.ForegroundColor = ConsoleColor.Green;
      HeadIdxModified.ForEach(file => Console.WriteLine($"\tmodified:\t{file}"));
      HeadIdxDeleted.ForEach(file => Console.WriteLine($"\tdeleted:\t{file}"));
      HeadIdxNew.ForEach(file => Console.WriteLine($"\tnew file:\t{file}"));
      Console.ResetColor();
      Console.WriteLine();
    }

    public static void PrintIndexAndWorkingDirectoryDiff()
    {
      if (IdxWkdModified.Count != 0 || IdxWkdDeleted.Count != 0)
      {
        Console.WriteLine("Changes not staged for commit");
        Console.ForegroundColor = ConsoleColor.Red;
        IdxWkdModified.ForEach(file => Console.WriteLine($"\tmodified:\t{file}"));
        IdxWkdDeleted.ForEach(file => Console.WriteLine($"\tdeleted:\t{file}"));
        Console.ResetColor();
        Console.WriteLine();
      }
      if (IdxWkdUntracked.Count != 0)
      {
        Console.WriteLine("Untracked files");
        Console.ForegroundColor = ConsoleColor.Red;
        IdxWkdUntracked.ForEach(file => Console.WriteLine($"\t{file}"));
        Console.ResetColor();
        Console.WriteLine();
      }
    }
  }
}
