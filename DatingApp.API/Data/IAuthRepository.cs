using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
  public interface IAuthRepository
  {
    // Register user
    Task<User> Register(User user, string password);

    // User login
    Task<User> Login(string username, string password);

    Task<bool> DoesUserExist(string username);
  }
}