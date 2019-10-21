using System.Threading.Tasks;

namespace DatingApp.API.Data
{
	public interface IBaseEntity
	{
		void Add<T>(T entity) where T : class;
		void Delete<T>(T entity) where T : class;
		Task<bool> SaveAll();
	}
}