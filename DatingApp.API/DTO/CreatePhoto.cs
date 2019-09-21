using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.DTO
{
	public class CreatePhoto
	{
		public string Url { get; set; }

		// handles files sent over http
		public IFormFile File { get; set; }

		public string Description { get; set; }

		public DateTime DateAdded { get; set; }

		public string PublicId { get; set; }

		public CreatePhoto()
		{
				DateAdded = DateTime.Now;
		}
	}
}