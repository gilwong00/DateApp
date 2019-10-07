using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
	public class DataContext : DbContext
	{
		/*
			The parameters for our constructor is DbContextOptions where it takes a 
			type of our class name. We then need to chain this to the base constructor 
			which is the DbContext class we inherit so then we use : and the keyword base and 
			pass in the options
		*/
		public DataContext(DbContextOptions<DataContext> options) : base(options) { }

		// Telling entity framework what our entities is
		// Each one of these DbSets will represent a table in our database
		// DbSet takes a type of what the data should look like
		public DbSet<Value> Values { get; set; }

		public DbSet<User> Users { get; set; }

		public DbSet<Photo> Photos { get; set; }

		public DbSet<Like> Likes { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Like>().HasKey(k => new { k.LikerId, k.LikeeId });

			// configuring likee and liker relationship. Likee can have multiple likers
			builder.Entity<Like>()
				.HasOne(u => u.Likee)
				.WithMany(u => u.Likers)
				.HasForeignKey(u => u.LikeeId)
				.OnDelete(DeleteBehavior.Restrict);

			// liker and likee relationship. A liker can like multiple people
			builder.Entity<Like>()
				.HasOne(u => u.Liker)
				.WithMany(u => u.Likees)
				.HasForeignKey(u => u.LikerId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}