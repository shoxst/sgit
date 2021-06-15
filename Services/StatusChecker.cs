using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace sgit
{
  public static class StatusChecker
  {
    private static List<string> headIdxModified = new List<string>();
    private static List<string> headIdxDeleted = new List<string>();
    private static List<string> headIdxNew = new List<string>();
    private static List<string> idxWkdModified = new List<string>();
    private static List<string> idxWkdDeleted = new List<string>();
    private static List<string> idxWkdUntracked = new List<string>();

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
          headIdxNew.Add(sgitFilePath);
        }
        return headIdxNew.Count != 0;
      }
      var head = ObjectReader.GetBlobsFromCommit(commit);
      var copy = new Dictionary<string, string>(head);

      foreach (var item in index)
      {
        if (!head.TryGetValue(item.Key, out var hash))
        {
          // Exists in index, not in head
          headIdxNew.Add(item.Key);
          continue;
        }
        if (item.Value != hash)
        {
          // File content is different between index and head
          headIdxModified.Add(item.Key);
        }
        copy.Remove(item.Key);
      }
      foreach (var sgitFilePath in copy.Keys)
      {
        // Exists in head, not in index
        headIdxDeleted.Add(sgitFilePath);
      }
      return headIdxModified.Count != 0 || headIdxDeleted.Count != 0 || headIdxNew.Count != 0;
    }

    public static bool CompareIndexAndWorkingDirectory()
    {
      var sgitFilePaths = PathUtil.GetAllFilesUnderPath(PathUtil.SGIT_REPOSITORY, true)
                        .Select(filePath => PathUtil.GetSgitFilePath(filePath));

      if (!File.Exists(PathUtil.SGIT_INDEX))
      {
        idxWkdUntracked.AddRange(sgitFilePaths);
        return idxWkdUntracked.Count != 0;
      }

      var index = Index.GetDictionary();
      var copy = new Dictionary<string, string>(index);

      foreach (var sgitFilePath in sgitFilePaths)
      {
        if (!index.TryGetValue(sgitFilePath, out var hash))
        {
          // Exists in working directory, not in index
          idxWkdUntracked.Add(sgitFilePath);
          continue;
        }
        var blob = new BlobObject(sgitFilePath);
        if (blob.Hash != hash)
        {
          // File content is different between working directory and index
          idxWkdModified.Add(sgitFilePath);
        }
        copy.Remove(sgitFilePath);
      }
      foreach (var sgitFilePath in copy.Keys)
      {
        // Exists in index, not in working directory
        idxWkdDeleted.Add(sgitFilePath);
      }
      return idxWkdModified.Count != 0 || idxWkdDeleted.Count != 0 || idxWkdUntracked.Count != 0;
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
      headIdxModified.ForEach(file => Console.WriteLine($"\tmodified:\t{file}"));
      headIdxDeleted.ForEach(file => Console.WriteLine($"\tdeleted:\t{file}"));
      headIdxNew.ForEach(file => Console.WriteLine($"\tnew file:\t{file}"));
      Console.ResetColor();
      Console.WriteLine();
    }

    public static void PrintIndexAndWorkingDirectoryDiff()
    {
      if (idxWkdModified.Count != 0 || idxWkdDeleted.Count != 0)
      {
        Console.WriteLine("Changes not staged for commit");
        Console.ForegroundColor = ConsoleColor.Red;
        idxWkdModified.ForEach(file => Console.WriteLine($"\tmodified:\t{file}"));
        idxWkdDeleted.ForEach(file => Console.WriteLine($"\tdeleted:\t{file}"));
        Console.ResetColor();
        Console.WriteLine();
      }
      if (idxWkdUntracked.Count != 0)
      {
        Console.WriteLine("Untracked files");
        Console.ForegroundColor = ConsoleColor.Red;
        idxWkdUntracked.ForEach(file => Console.WriteLine($"\t{file}"));
        Console.ResetColor();
        Console.WriteLine();
      }
    }
  }
}
