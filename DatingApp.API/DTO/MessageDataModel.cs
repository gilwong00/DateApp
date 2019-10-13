using System;

namespace DatingApp.API.DTO
{
	public class MessageDataModel
	{
		public int SenderId { get; set; }

		public int RecipientId { get; set; }

		public DateTime DateSent { get; set; }

		public string Content { get; set; }

		public MessageDataModel()
		{
			DateSent = DateTime.Now;
		}
	}
}