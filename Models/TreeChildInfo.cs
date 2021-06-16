namespace sgit
{
  public class TreeChildInfo
  {
    public string Mode { get; set; }
    public ObjectType Type { get; set; }
    public string Name { get; set; }
    public string Hash { get; set; }

    public TreeChildInfo(string mode, ObjectType type, string name, string hash)
    {
      Mode = mode;
      Type = type;
      Name = name;
      Hash = hash;
    }
  }
}
