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
		public DataContext(DbContextOptions<DataContext> options) : base(options) {}

		// Telling entity framework what our entities is
		// Each one of these DbSets will represent a table in our database
		// DbSet takes a type of what the data should look like
		public DbSet<Value> Values { get; set; }
	}
}