using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
	//[Authorize]
	[Route("/api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthRepository _repo;
		private readonly IConfiguration _config;
		public AuthController(IAuthRepository repo, IConfiguration config)
		{
			_config = config;
			_repo = repo;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterUserDTO user)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

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

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginUserDTO user)
		{
			var dbUser = await _repo.Login(user.Username.ToLower(), user.Password.ToLower());

			if (dbUser == null)
			{
				return Unauthorized();
			}

			// token will contain id and username
			var claims = new[]
			{
				// id
				new Claim(ClaimTypes.NameIdentifier, dbUser.Id.ToString()),
				new Claim(ClaimTypes.Name, dbUser.Username)
			};

			// token secret
			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(
					_config.GetSection("AppSettings:Token").Value
				)
			);

			// signing credentials, we also make sure we use the token secret apart of the sigining credentials
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
			
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddDays(1),
				SigningCredentials = credentials
			};

			var tokenHandler = new JwtSecurityTokenHandler();

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return Ok(new {
				token = tokenHandler.WriteToken(token)
			});
		}
	}
}