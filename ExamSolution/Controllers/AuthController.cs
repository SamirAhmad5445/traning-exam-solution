using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Security.Cryptography;

namespace ExamSolution.Controllers
{
  [Route("api")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    [HttpPost("Login")]
    public ActionResult<string> Login(string name)
    {
      try
      {
        if (string.IsNullOrEmpty(name))
        {
          return BadRequest("Invalid Username");
        }

        var user = Helpers.GetUser(name);

        if (user == null)
        {
          RemoveCookies();
          return NotFound("User was not found.");
        }

        SetCookies(user);
        return Ok("Done.");
      }
      catch (Exception)
      {
        return StatusCode(500, "An error occured while processing your request.");
      }
    }

    private void SetCookies(User user)
    {
      Response.Cookies.Append("username", user.Name);
      Response.Cookies.Append("token", user.Token);
      Response.Cookies.Append("role", user.Role);
    }

    private void RemoveCookies()
    {
      Response.Cookies.Delete("username");
      Response.Cookies.Delete("role");
      Response.Cookies.Delete("token");
    }
  }
}
