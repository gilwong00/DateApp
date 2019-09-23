using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.API.Helpers
{
	public class LogUserActivity : IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			// httpContext is the context of the http request. 
			// We can access services like our Datingrepository
			// or get access to the User making the request
			
			var httpContext = await next(); // await till the action has been completed
			var userId = int.Parse(httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
			var repo = httpContext.HttpContext.RequestServices.GetService<IDatingRepository>();
			var user = await repo.GetUser(userId);
			user.LastActive = DateTime.Now;
			await repo.SaveAll();
		}
	}
}