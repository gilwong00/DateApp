using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
	public interface IUserRepository : IBaseEntity
	{
		Task<PagedList<User>> GetUsers(UserParams userParams);

		Task<User> GetUser(int id);

		Task<Photo> GetMainUserPhoto(int userId);

		Task<Photo> GetPhoto(int id);

		Task<Like> GetLike(int userId, int recipientId);
	}
}