using System.IO;

namespace sgit
{
  public static class PathUtil
  {
    public static readonly string SGIT_REPOSITORY = "work";
    public static readonly string SGIT_DOTSGIT = $"{SGIT_REPOSITORY}/.sgit";
    public static readonly string SGIT_INDEX = $"{SGIT_DOTSGIT}/index";
    public static readonly string SGIT_HEAD = $"{SGIT_DOTSGIT}/head";
    public static readonly string SGIT_OBJECTS = $"{SGIT_DOTSGIT}/objects";
    public static readonly string SGIT_REFS = $"{SGIT_DOTSGIT}/refs";
    public static readonly string SGIT_REFS_HEADS = $"{SGIT_DOTSGIT}/refs/heads";
    public static readonly string SGIT_REFS_TAGS = $"{SGIT_DOTSGIT}/refs/tags";

    public static string GetSgitFilePath(string filePath)
      => filePath.Remove(0, SGIT_REPOSITORY.Length + 1);

    public static string GetFilePath(string sgitFilePath)
      => Path.Combine(SGIT_REPOSITORY, sgitFilePath);
  }
}