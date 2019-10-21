using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
	public class MessageRepository : BaseEntity, IMessageRepository
	{
		private readonly DataContext _context;
		public MessageRepository(DataContext context) : base(context)
		{
			_context = context;
		}

		public async Task<Message> GetMessage(int messageId)
		{
			return await _context.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
		}

		public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
		{
			var messages = _context.Messages
				.Include(u => u.Sender) // get fully hydrated Sender
					.ThenInclude(p => p.Photos) // get photo for sender
				.Include(u => u.Recipient) // get fully hydrated recipient
					.ThenInclude(p => p.Photos) // get photo for recipient
				.AsQueryable();

			switch (messageParams.MessageContainer)
			{
				// messages a user has received
				case "Inbox":
					messages = messages.Where(m => m.RecipientId == messageParams.UserId && m.RecipientDeleted == false);
					break;
				case "Outbox":
					messages = messages.Where(m => m.SenderId == messageParams.UserId && m.SenderDeleted == false);
					break;
				default:
					messages = messages.Where(m => m.RecipientId == messageParams.UserId &&
						m.RecipientDeleted == false &&
						m.isRead == false
					);
					break;
			}

			messages = messages.OrderByDescending(m => m.DateSent);
			return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
		}

		public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
		{
			var messages = await _context.Messages
				.Include(u => u.Sender)
					.ThenInclude(p => p.Photos)
				.Include(u => u.Recipient)
					.ThenInclude(p => p.Photos)
				// Filteriing a convo between 2 users
				.Where(
					m => m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId ||
					m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted == false)
				.OrderByDescending(m => m.DateSent)
				.ToListAsync();

			return messages;
		}
	}
}