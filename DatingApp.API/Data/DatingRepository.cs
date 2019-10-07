using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
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

		public async Task<PagedList<User>> GetUsers(UserParams userParams)
		{
			var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
			users = users
				.Where(u => u.Id != userParams.UserId)
				.Where(x => x.Gender == userParams.Gender);

			if (userParams.Likers)
			{
				var likers = await GetUserLikes(userParams.UserId, userParams.Likers);
				users = users.Where(u => likers.Contains(u.Id)); // filtering out people that has liked the logged in users
			}

			if (userParams.Likees)
			{
				var likees = await GetUserLikes(userParams.UserId, userParams.Likers);
				users = users.Where(u => likees.Contains(u.Id)); // filtering out users the current logged in user has liked
			}

			// Age filtering. If min and or max age is specified
			if (userParams.MinAge != 18 || userParams.MaxAge != 99)
			{
				var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
				var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
				users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
			};

			if (!string.IsNullOrEmpty(userParams.OrderBy))
			{
				switch (userParams.OrderBy)
				{
					case "created":
						users = users.OrderByDescending(u => u.Created);
						break;
					default:
						users = users.OrderByDescending(u => u.LastActive);
						break;
				}
			};

			return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
		}

		// returns list of likers or likees
		private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
		{
			var user = await _context.Users
				.Include(x => x.Likers)
				.Include(x => x.Likees)
				.FirstOrDefaultAsync(u => u.Id == id);

			// if likers get list of users that has liked the logged in user
			if (likers)
			{
				return user.Likers
					.Where(u => u.LikeeId == id) // get list of user the logged in user has liked
					.Select(i => i.LikerId); // returns list of people that likes the logged in user
			}
			else
			{
				// get list of people the logged in user has liked
				return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
			}
		}

		public async Task<bool> SaveAll()
		{
			// if SaveChangesAsync returns 0 that means no changes were saved
			// SaveChangesAsync will return the number of changes saved
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<Photo> GetPhoto(int id)
		{
			var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
			return photo;
		}

		public async Task<Photo> GetMainUserPhoto(int userId)
		{
			return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
		}

		public async Task<Like> GetLike(int userId, int recipientId)
		{
			return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
		}
	}
}