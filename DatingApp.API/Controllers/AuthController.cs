using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
  [Route("/api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IAuthRepository _repo;
    public AuthController(IAuthRepository repo)
    {
      _repo = repo;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDTO user)
    {
      // validate request

      user.Username = user.Username.ToLower();

      if (await _repo.DoesUserExist(user.Username))
      {
        return BadRequest("Username already exist");
      }

      var newUser = new User
      {
        Username = user.Username
      };

      var createdUser = _repo.Register(newUser, user.Password);
      return StatusCode(201);
    }
  }
}