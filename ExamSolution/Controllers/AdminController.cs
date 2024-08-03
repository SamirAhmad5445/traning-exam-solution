using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExamSolution.Controllers
{
  [Route("api/admin")]
  [ApiController]
  public class AdminController : ControllerBase
  {
    [HttpPost("add-user")]
    public ActionResult<string> AddUser(string name)
    {
      if (!IsAdminAllowed())
      {
        return Unauthorized();
      }

      try
      {
        var usersList = Helpers.GetAllUsers()!;

        if (usersList.FirstOrDefault(u => u.Name == name) != null) {
          return Conflict();
        }

        usersList.Add(new User(name, "user"));

        System.IO.File.WriteAllText("data.json", JsonConvert.SerializeObject(usersList));

        return Ok("New user has been added.");
      }
      catch (Exception)
      {
        return StatusCode(500, "An error occured while processing your request.");
      }
    }

    private bool IsAdminAllowed()
    {
      string? username = Request.Cookies["username"];
      string? role = Request.Cookies["role"];
      string? token = Request.Cookies["token"];

      if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(token))
      {
        return false;
      }

      var user = Helpers.GetUser(username);

      if (user == null)
      {
        return false;
      }

      return user.Role == "admin" && user.Token == token;
    }
  }
}
