using System;

namespace DatingApp.API.DTO
{
	public class PhotoResponse
	{
		public int Id { get; set; }

		public string Url { get; set; }

		public string Description { get; set; }

		public DateTime DateAdded { get; set; }

		public bool IsMain { get; set; }

		public string PublicId { get; set; }
	}
}