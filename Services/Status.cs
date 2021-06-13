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
      if (CompareHeadAndIndex() == 0 && CompareWorkingDirectoryAndIndex() == 0)
      {
        Console.WriteLine($"On branch {Reference.GetHead()}");
        Console.WriteLine("nothing to commit, working tree clean");
      }
    }

    public static int CompareHeadAndIndex()
    {
      return 0;
    }

    private static int CompareWorkingDirectoryAndIndex()
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
        return 1;
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
        return 0;
      }
      PrintWorkingDirectoryAndIndexInfo(modifiedFiles, deletedFiles, untrackedFiles);
      return 1;
    }

    private static void PrintWorkingDirectoryAndIndexInfo(
      List<string> modifiedFiles, List<string> deletedFiles, List<string> untrackedFiles)
    {
      if (modifiedFiles.Count != 0 || deletedFiles.Count != 0)
      {
        Console.WriteLine("Changes not staged for commit");
        modifiedFiles.ForEach(file => Console.WriteLine($"\tmodified:\t{file}"));
        deletedFiles.ForEach(file => Console.WriteLine($"\tdeleted:\t{file}"));
        Console.WriteLine();
      }
      if (untrackedFiles.Count != 0)
      {
        Console.WriteLine("Untracked files");
        untrackedFiles.ForEach(file => Console.WriteLine($"\t{file}"));
        Console.WriteLine();
      }
    }
  }
}
