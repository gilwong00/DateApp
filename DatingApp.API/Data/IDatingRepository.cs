using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
	public interface IDatingRepository
	{
		/*
			Add a type of T, T is either a user or photo class and we constrain
			this by doing where T: class
		 */
		void Add<T>(T entity) where T : class;

		void Delete<T>(T entity) where T : class;

		Task<bool> SaveAll();

		Task<IEnumerable<User>> GetUsers();

		Task<User> GetUser(int id);
	}
}