using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace sgit
{
  public static class Reference
  {
    // HEAD
    public static string GetHead() =>
      File.ReadAllText(PathUtil.SGIT_HEAD).Replace("ref: refs/heads/", "").Replace("\n", "");

    public static void SetHead(string branchName) =>
      File.WriteAllText(PathUtil.SGIT_HEAD, $"ref: refs/heads/{branchName}\n");
    
    // heads
    public static string GetCommitByBranch(string branchName)
    {
      var filePath = Path.Combine(PathUtil.SGIT_REFS_HEADS, branchName);
      if (!File.Exists(filePath))
      {
        return null;
      }
      var str = File.ReadAllText(filePath);
      return str.Replace("\n", "");
    }

    public static void SetCommitForBranch(string branchName, string commit) => 
      File.WriteAllText(Path.Combine(PathUtil.SGIT_REFS_HEADS, branchName), $"{commit}\n");

    public static string GetHeadCommit() =>
      GetCommitByBranch(GetHead());
    
    public static void SetHeadCommit(string commit) => 
      SetCommitForBranch(GetHead(), commit);

    public static List<string> GetAllBranchNames() =>
      Directory.EnumerateFiles(PathUtil.SGIT_REFS_HEADS).Select(filePath => Path.GetFileName(filePath)).ToList();
    
    public static void DeleteBranch(string branchName) =>
      File.Delete(Path.Combine(PathUtil.SGIT_REFS_HEADS, branchName));
    
    // tags
    public static string GetHashByTagName(string tagName)
    {
      var filePath = Path.Combine(PathUtil.SGIT_REFS_TAGS, tagName);
      if (!File.Exists(filePath))
      {
        return null;
      }
      var str = File.ReadAllText(filePath);
      return str.Replace("\n", "");
    }

    public static void SetHashForTag(string tagName, string hash) => 
      File.WriteAllText(Path.Combine(PathUtil.SGIT_REFS_TAGS, tagName), $"{hash}\n");

    public static List<string> GetAllTagNames() =>
      Directory.EnumerateFiles(PathUtil.SGIT_REFS_TAGS).Select(filePath => Path.GetFileName(filePath)).ToList();
    
    public static void DeleteTag(string tagName) =>
      File.Delete(Path.Combine(PathUtil.SGIT_REFS_TAGS, tagName));
  }
}
