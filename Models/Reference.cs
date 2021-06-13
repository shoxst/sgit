using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace sgit
{
  public static class Reference
  {
    public static string GetCommit(string branchName)
    {
      var filePath = Path.Combine(PathUtil.SGIT_REFS_HEADS, branchName);
      if (!File.Exists(filePath))
      {
        return null;
      }
      var str = File.ReadAllText(filePath);
      return str.Replace("\n", "");
    }

    public static string GetHeadCommit() =>
      GetCommit(GetHead());

    public static void SetCommit(string branchName, string commit) => 
      File.WriteAllText(Path.Combine(PathUtil.SGIT_REFS_HEADS, branchName), $"{commit}\n");

    public static void SetHeadCommit(string commit) => 
      SetCommit(GetHead(), commit);
    
    public static string GetHead() =>
      File.ReadAllText(PathUtil.SGIT_HEAD).Replace("ref: refs/heads/", "").Replace("\n", "");

    public static void SetHead(string branchName) =>
      File.WriteAllText(PathUtil.SGIT_HEAD, $"ref: refs/heads/{branchName}\n");
    
    public static List<string> GetAllBranchNames() =>
      Directory.EnumerateFiles(PathUtil.SGIT_REFS_HEADS).Select(filePath => Path.GetFileName(filePath)).ToList();
    
    public static void DeleteBranch(string branchName) =>
      File.Delete(Path.Combine(PathUtil.SGIT_REFS_HEADS, branchName));
  }
}
