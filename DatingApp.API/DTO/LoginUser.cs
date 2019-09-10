using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTO
{
	public class LoginUser
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }
	}
}