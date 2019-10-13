using System;

namespace DatingApp.API.DTO
{
	public class MessageViewModel
	{
		public int Id { get; set; }

		public int SenderId { get; set; }

		// this will auto populate. AutoMapper will see the senderId and finds a user and takes the knownAs from the User Model
		public string SenderKnownAs { get; set; }

		public string SenderPhotoUrl { get; set; }

		public int RecipientId { get; set; }

		public string RecipientKnownAs { get; set; }

		public string RecipientPhotoUrl { get; set; }

		public string Content { get; set; }

		public bool isRead { get; set; }

		public DateTime? DateRead { get; set; }

		public DateTime DateSent { get; set; }
	}
}