using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace sgit
{
  public static class Status
  {
    public static void Exec()
    {
      bool diffHeadAndIndex = CompareHeadAndIndex();
      bool diffWorkingDirectoryAndIndex = CompareWorkingDirectoryAndIndex();
      if (!diffHeadAndIndex && !diffWorkingDirectoryAndIndex)
      {
        Console.WriteLine($"On branch {Reference.GetHead()}");
        Console.WriteLine("nothing to commit, working tree clean");
      }
    }

    public static bool CompareHeadAndIndex()
    {
      var newFiles = new List<string>();
      var modifiedFiles = new List<string>();
      var deletedFiles = new List<string>();
      
      if (!File.Exists(PathUtil.SGIT_INDEX))
      {
        return false;
      }
      // index
      var index = Index.GetDictionary();

      // head
      var commit = Reference.GetHeadCommit();
      var head = ObjectReader.GetBlobsFromCommit(commit);
      var copy = new Dictionary<string, string>(head);

      foreach (var item in index)
      {
        if (!head.TryGetValue(item.Key, out var hash))
        {
          // Exists in index, not in head
          newFiles.Add(item.Key);
          continue;
        }
        if (item.Value != hash)
        {
          // File content is different between index and head
          modifiedFiles.Add(item.Key);
        }
        copy.Remove(item.Key);
      }
      foreach (var sgitFilePath in copy.Keys)
      {
        // Exists in head, not in index
        deletedFiles.Add(sgitFilePath);
      }

      if (modifiedFiles.Count == 0 && deletedFiles.Count == 0 && newFiles.Count == 0)
      {
        return false;
      }
      PrintIndexAndHeadInfo(modifiedFiles, deletedFiles, newFiles);
      return true;
    }

    private static void PrintIndexAndHeadInfo(
       List<string> modifiedFiles, List<string> deletedFiles, List<string> newFiles)
    {
      Console.WriteLine("Changes to be commited");
      Console.ForegroundColor = ConsoleColor.Green;
      modifiedFiles.ForEach(file => Console.WriteLine($"\tmodified:\t{file}"));
      deletedFiles.ForEach(file => Console.WriteLine($"\tdeleted:\t{file}"));
      newFiles.ForEach(file => Console.WriteLine($"\tnew file:\t{file}"));
      Console.WriteLine();
      Console.ResetColor();
    }

    private static bool CompareWorkingDirectoryAndIndex()
    {
      var modifiedFiles = new List<string>();
      var deletedFiles = new List<string>();
      var untrackedFiles = new List<string>();
      
      // working directory
      var sgitFilePaths = PathUtil.GetAllFilesUnderPath(PathUtil.SGIT_REPOSITORY, true)
                        .Select(filePath => PathUtil.GetSgitFilePath(filePath));

      if (!File.Exists(PathUtil.SGIT_INDEX))
      {
        untrackedFiles.AddRange(sgitFilePaths);
        PrintWorkingDirectoryAndIndexInfo(modifiedFiles, deletedFiles, untrackedFiles);
        return true;
      }

      // index
      var index = Index.GetDictionary();
      var copy = new Dictionary<string, string>(index);

      foreach (var sgitFilePath in sgitFilePaths)
      {
        if (!index.TryGetValue(sgitFilePath, out var hash))
        {
          // Exists in working directory, not in index
          untrackedFiles.Add(sgitFilePath);
          continue;
        }
        var blob = new BlobObject(sgitFilePath);
        if (blob.Hash != hash)
        {
          // File content is different between working directory and index
          modifiedFiles.Add(sgitFilePath);
        }
        copy.Remove(sgitFilePath);
      }
      foreach (var sgitFilePath in copy.Keys)
      {
        // Exists in index, not in working directory
        deletedFiles.Add(sgitFilePath);
      }

      if (modifiedFiles.Count == 0 && deletedFiles.Count == 0 && untrackedFiles.Count == 0)
      {
        return false;
      }
      PrintWorkingDirectoryAndIndexInfo(modifiedFiles, deletedFiles, untrackedFiles);
      return true;
    }

    private static void PrintWorkingDirectoryAndIndexInfo(
      List<string> modifiedFiles, List<string> deletedFiles, List<string> untrackedFiles)
    {
      if (modifiedFiles.Count != 0 || deletedFiles.Count != 0)
      {
        Console.WriteLine("Changes not staged for commit");
        Console.ForegroundColor = ConsoleColor.Red;
        modifiedFiles.ForEach(file => Console.WriteLine($"\tmodified:\t{file}"));
        deletedFiles.ForEach(file => Console.WriteLine($"\tdeleted:\t{file}"));
        Console.ResetColor();
        Console.WriteLine();
      }
      if (untrackedFiles.Count != 0)
      {
        Console.WriteLine("Untracked files");
        Console.ForegroundColor = ConsoleColor.Red;
        untrackedFiles.ForEach(file => Console.WriteLine($"\t{file}"));
        Console.ResetColor();
        Console.WriteLine();
      }
    }
  }
}
