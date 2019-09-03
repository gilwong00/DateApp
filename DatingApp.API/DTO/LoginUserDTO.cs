using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTO
{
	public class LoginUserDTO
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }
	}
}