using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace ExamSolution.Controllers
{
  [Route("api/user")]
  [ApiController]
  public class UserController(IWebHostEnvironment env) : ControllerBase
  {
    private readonly string UsersDirectory = "UsersDataFiles";

    [HttpPost("data")]
    public ActionResult<string> CreateData(string data)
    {
      if(!IsUserAllowed())
      {
        return Unauthorized();
      }

      try
      {
        if (data == null)
        {
          return BadRequest("Data cant be null.");
        }

        string username = Request.Cookies["username"]!;
        string rootPath = env.ContentRootPath;

        string UploadsFolder = Path.Combine(rootPath, UsersDirectory);
        string filePath = Path.Combine(UploadsFolder, $"{username}.txt");

        if (!Directory.Exists(UploadsFolder))
        {
          Directory.CreateDirectory(UploadsFolder);
        }



        System.IO.File.WriteAllText(filePath, data);

        return Ok("Done.");
      }
      catch (Exception)
      {
        return StatusCode(500, "An error occured while processing your request.");
      }
    }

    [HttpGet("register-ticket")]
    public ActionResult<string> GetTicket()
    {
      if (!IsUserAllowed())
      {
        return Unauthorized();
      }

      try
      {
        string name = Request.Cookies["username"]!;

        if(!string.IsNullOrEmpty(Helpers.GetUser(name)!.Ticket))
        {
          return BadRequest();
        }

        var usersList = Helpers.GetAllUsers()!;

        usersList.ForEach(u =>
        {
          if (u.Name != name) return;

          u.Ticket = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        });

        System.IO.File.WriteAllText("data.json", JsonConvert.SerializeObject(usersList));
        return Ok();
      }
      catch (Exception)
      {
        return StatusCode(500, "An error occured while processing your request.");
      }
    }

    [HttpGet("data")]
    public IActionResult GetData()
    {
      if (!IsUserAllowed())
      {
        return Unauthorized();
      }

      try
      {
        string name = Request.Cookies["username"]!;

        var user = Helpers.GetUser(name)!;

        if (string.IsNullOrEmpty(user.Ticket))
        {
          return BadRequest("You need to purchase a tiket first");
        }

        var usersList = Helpers.GetAllUsers()!;

        usersList.ForEach(u =>
        {
          if (u.Name != name) return;

          u.Ticket = string.Empty;
        });

        System.IO.File.WriteAllText("data.json", JsonConvert.SerializeObject(usersList));

        var filePath = Path.Combine(env.ContentRootPath, UsersDirectory, $"{name}.txt");

        if (!System.IO.File.Exists(filePath))
        {
          return NotFound("Your file was not found.");
        }

        return PhysicalFile(filePath, "text/html", $"{name}.txt");
      }
      catch (Exception)
      {
        return StatusCode(500, "An error occured while processing your request.");
      }
    }

    private bool IsUserAllowed()
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

      return user.Role == "user" && user.Token == token;
    }
  }
}
