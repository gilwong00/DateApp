namespace DatingApp.API.Helpers
{
	public class UserParams
	{
		private const int MAX_PAGE_SIZE = 20;
		public int PageNumber { get; set; } = 1; // default to 1

		private int pageSize = 5; // default pageSuze if we get nothing from the client
		public int PageSize
		{
			get { return pageSize; }
			// if client request a per page value created than ourt constant, return the max size
			// if not return the request value
			set { pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value; }
		}

		public int UserId { get; set; }

		public string Gender { get; set; }

		public int MinAge { get; set; } = 18;

		public int MaxAge { get; set; } = 99;

		public string OrderBy { get; set; }

		public bool Likees { get; set; } = false;

		public bool Likers { get; set; } = false;
	}
}