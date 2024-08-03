using Newtonsoft.Json;
using System.Security.Cryptography;

namespace ExamSolution
{
  public static class Helpers
  {
    public static List<User>? GetAllUsers()
    {
      var data = System.IO.File.ReadAllText("data.json");
      var users = JsonConvert.DeserializeObject<List<User>>(data);
      return users;
    }

    public static User? GetUser(string name)
    {
      var users = GetAllUsers();

      if (users == null)
      {
        return null;
      }

      return users.SingleOrDefault(u => u.Name == name);
    }
  }
}
