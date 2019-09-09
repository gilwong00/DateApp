using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
  public class AuthRepository : IAuthRepository
  {
    private readonly DataContext _context;

    public AuthRepository(DataContext context)
    {
      _context = context;
    }

    public async Task<bool> DoesUserExist(string username)
    {
      if (await _context.Users.AnyAsync(x => x.Username == username))
      {
        return true;
      }
      return false;
    }

    public async Task<User> Login(string username, string password)
    {
      var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

      if (user == null)
      {
        return null;
      }

      if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
      {
        return null; // should return 401 unauth
      }
      return user;
    }

    public async Task<User> Register(User user, string password)
    {
      byte[] passwordHash;
      byte[] passwordSalt;

      /* 
        we use the out keyword to pass the actually reference
        of passwordHash and passwordSalt. That way, when those variables
        are updated in the CreatePasswordHash method, it will update
        the variables in this scope as well
      */
      CreatePasswordHash(password, out passwordHash, out passwordSalt);
      user.PasswordHash = passwordHash;
      user.PasswordSalt = passwordSalt;

      await _context.Users.AddAsync(user);
      await _context.SaveChangesAsync();
      return user;
    }


		// using the out keyword for passwordHash and passwordSalt means we updated the passed in reference
		// dont need to return, the original reference gets updated
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (var salt = new System.Security.Cryptography.HMACSHA512())
      {
        passwordSalt = salt.Key;
        // computes password string into bytes
        passwordHash = salt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      }
    }

    private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
    {
      using (var salt = new System.Security.Cryptography.HMACSHA512(passwordSalt))
      {
        var computedHash = salt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        for (int i = 0; i < computedHash.Length; i++)
        {
          if (computedHash[i] != passwordHash[i])
          {
            return false;
          }
        }
      }
      return true;
    }
  }
}