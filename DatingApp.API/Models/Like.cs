namespace DatingApp.API.Models
{
	public class Like
	{
		// user liking Id
		public int LikerId { get; set; }

		// user being liked Id
		public int LikeeId { get; set; }

		public User Liker { get; set; }

		public User Likee { get; set; }
	}
}