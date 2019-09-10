using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
	public class DatingRepository : IDatingRepository
	{
		private readonly DataContext _context;
		public DatingRepository(DataContext context)
		{
			_context = context;
		}

		public void Add<T>(T entity) where T : class
		{
			// When we add, this will be saved in memory until we actually save our changes to our DB
			_context.Add(entity);
		}

		public void Delete<T>(T entity) where T : class
		{
			_context.Remove(entity);
		}

		public async Task<User> GetUser(int id)
		{
			// User Includes to tell our DB to also return the photos for the user
			var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
			return user;
		}

		public async Task<IEnumerable<User>> GetUsers()
		{
			var users = await _context.Users.Include(p => p.Photos).ToListAsync();
			return users;
		}

		public async Task<bool> SaveAll()
		{
			// if SaveChangesAsync returns 0 that means no changes were saved
			// SaveChangesAsync will return the number of changes saved
			return await _context.SaveChangesAsync() > 0;
		}
	}
}