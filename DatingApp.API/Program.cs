using System;
using DatingApp.API.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DatingApp.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateWebHostBuilder(args).Build();
			
			// this will seed the database if its empty on startup
			using(var scope = host.Services.CreateScope())
			{
				// getting all ServiceProviders in the app
				var services = scope.ServiceProvider;
				try 
				{
					var context = services.GetRequiredService<DataContext>();
					context.Database.Migrate();
					Seed.SeedUsers(context);
				}
				catch(Exception err)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(err, "Error occured during migration");
				}
			}
			host.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
				WebHost.CreateDefaultBuilder(args)
						.UseStartup<Startup>();
	}
}
