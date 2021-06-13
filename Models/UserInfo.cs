namespace sgit
{
  public class UserInfo
  {
    public string Name { get; set; }
    public string Email { get; set; }
    public int DateSeconds { get; set; }
    public string DateTimezone { get; set; }

    public UserInfo(string name, string email, int dateSeconds, string dateTimezone)
    {
      Name = name;
      Email = email;
      DateSeconds = dateSeconds;
      DateTimezone = dateTimezone;
    }
  }
}
