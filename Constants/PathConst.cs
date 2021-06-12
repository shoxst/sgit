namespace sgit
{
  public static class PathConst
  {
    public static string SGIT_REPOSITORY = "work";
    public static string SGIT_DOTSGIT = $"{SGIT_REPOSITORY}/.sgit";
    public static string SGIT_INDEX = $"{SGIT_DOTSGIT}/index";
    public static string SGIT_HEAD = $"{SGIT_DOTSGIT}/head";
    public static string SGIT_OBJECTS = $"{SGIT_DOTSGIT}/objects";
    public static string SGIT_REFS = $"{SGIT_DOTSGIT}/refs";
    public static string SGIT_REFS_HEADS = $"{SGIT_DOTSGIT}/refs/heads";
    public static string SGIT_REFS_TAGS = $"{SGIT_DOTSGIT}/refs/tags";
  }
}
