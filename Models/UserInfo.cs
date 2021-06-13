using System;

namespace sgit
{
  public class UserInfo
  {
    public string Name { get; set; }
    public string Email { get; set; }
    public int DateSeconds { get; set; }
    public string DateTimezone { get; set; }

    public UserInfo(string name, string email, int dateSeconds)
    {
      Name = name;
      Email = email;
      DateSeconds = dateSeconds;
      DateTimezone = TimeZoneInfo.Local.DisplayName.Substring(4,6).Replace(":","");
    }
  }
}
