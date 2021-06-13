using System.IO;

namespace sgit
{
  public static class Reference
  {
    public static string GetCommit(string branchName)
    {
      var filePath = $"{PathUtil.SGIT_REFS_HEADS}/{branchName}";
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
      File.WriteAllText($"{PathUtil.SGIT_REFS_HEADS}/{branchName}", $"{commit}\n");

    public static void SetHeadCommit(string commit) => 
      SetCommit(GetHead(), commit);
    
    public static string GetHead() =>
      File.ReadAllText(PathUtil.SGIT_HEAD).Replace("ref: refs/heads/", "").Replace("\n", "");

    public static void SetHead(string branchName) =>
      File.WriteAllText(PathUtil.SGIT_HEAD, $"ref: refs/heads/{branchName}\n");
  }
}