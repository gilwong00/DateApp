using System;

namespace DatingApp.API.Models
{
	public class Message
	{
		public int Id { get; set; }

		public int SenderId { get; set; }

		public User Sender { get; set; }

		public int RecipientId { get; set; }

		public User Recipient { get; set; }

		public string Content { get; set; }

		public bool isRead { get; set; }

		public DateTime? DateRead { get; set; }

		public DateTime DateSent { get; set; }

		public bool SenderDeleted { get; set; }

		public bool RecipientDeleted { get; set; }
	}
}