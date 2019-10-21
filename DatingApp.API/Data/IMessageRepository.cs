using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
	public interface IMessageRepository : IBaseEntity
	{
		Task<Message> GetMessage(int messageId);
		Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
		Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
	}
}