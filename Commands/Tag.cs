using System;
using System.Linq;

namespace sgit
{
  public static class Tag
  {
    public static void Exec(string[] args)
    {
      // `sgit tag`
      if (args.Length == 1)
      {
        ListTagNames();
        return;
      }
      // `sgit tag {name}`
      if (args.Length == 2)
      {
        CreateLightweightTag(args[1]);
        return;
      }
      // `sgit tag -a {name} -m "{message}"
      if (args.Length == 5 && args[1] == "-a" && args[3] == "-m")
      {
        string message = args[4].Trim('\"')+'\n';
        CreateAnnotatedTag(args[2], message);
      }
      // 'sgit tag -d {name}'
      if (args.Length == 3 && args[1] == "-d")
      {
        DeleteTag(args[2]);
        return;
      }
    }

    private static void ListTagNames()
    {
      Reference.GetAllTagNames().ForEach(tag => Console.WriteLine(tag));
    }

    private static void CreateLightweightTag(string name)
    {
      if (Reference.GetAllTagNames().Any(t => t == name))
      {
        Console.WriteLine($"tag '{name}' already exists");
        return;
      }
      var head = Reference.GetHead();
      var commit = Reference.GetHeadCommit();
      if (commit == null)
      {
        Console.WriteLine($"Not a valid reference name: '{head}'");
        return;
      }
      Reference.SetHashForTag(name, commit);
    }

    private static void CreateAnnotatedTag(string name, string message)
    {
      var currentCommit = Reference.GetHeadCommit();
      var seconds = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
      var timezone = TimeZoneInfo.Local.DisplayName.Substring(4,6).Replace(":","");
      var tagger = new UserInfo("*****", "*****", seconds, timezone);
      var tag = new TagObject(currentCommit, ObjectType.commit, name, tagger, message);
      var hash = tag.Write();
      Reference.SetHashForTag(name, hash);
    }

    private static void DeleteTag(string name)
    {
      if (!Reference.GetAllTagNames().Any(t => t == name))
      {
        Console.WriteLine($"tag '{name}' not found");
        return;
      }
      Reference.DeleteTag(name);
      Console.WriteLine($"Deleted tag {name}");
    }
  }
}
