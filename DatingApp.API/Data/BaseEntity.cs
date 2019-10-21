using System.Threading.Tasks;

namespace DatingApp.API.Data
{
	public class BaseEntity : IBaseEntity
	{
		private readonly DataContext _context;

		public BaseEntity(DataContext context)
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

		public async Task<bool> SaveAll()
		{
			// if SaveChangesAsync returns 0 that means no changes were saved
			// SaveChangesAsync will return the number of changes saved
			return await _context.SaveChangesAsync() > 0;
		}
	}
}