using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
	public class Seed
	{
		public static void SeedUsers(DataContext context)
		{
			// if no users in DB
			if (!context.Users.Any())
			{
				var userData = System.IO.File.ReadAllText("Data/UserDataSeed.json");
				// converts json records into a list of users
				var users = JsonConvert.DeserializeObject<List<User>>(userData);

				// users.Select(user => {
				// 	byte[] passwordHash, passwordSalt;
				// 	CreatePasswordHash("password", out passwordHash, out passwordSalt);
				// 	user.PasswordHash = passwordHash;
				// 	user.PasswordSalt = passwordSalt;
				// 	user.Username = user.Username.ToLower();
				// 	context.Users.Add(user);
				// 	return user;
				// });

				foreach (var user in users)
				{
					byte[] passwordHash, passwordSalt;
					CreatePasswordHash("password", out passwordHash, out passwordSalt);
					user.PasswordHash = passwordHash;
					user.PasswordSalt = passwordSalt;
					user.Username = user.Username.ToLower();
					context.Users.Add(user);
				}

				context.SaveChanges();
			}
		}

		private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var salt = new System.Security.Cryptography.HMACSHA512())
			{
				passwordSalt = salt.Key;
				// computes password string into bytes
				passwordHash = salt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}
	}
}