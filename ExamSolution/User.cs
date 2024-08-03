using System.Security.Cryptography;

namespace ExamSolution
{
  public class User
  {
    public User(string name, string role)
    {
      Name = name;
      Role = role;
      Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
      Ticket = string.Empty;
    }

    public string Name { get; set; } 
    public string Role { get; set; } 
    public string Token { get; set; }
    public string Ticket { get; set; }

  }
}
