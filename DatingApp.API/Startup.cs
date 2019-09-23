using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				.AddJsonOptions(opts =>
				{
					// Ignores self rep loop error
					opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
				});
			services.AddCors();
			// gets setting from appsettings and binding it to our CloudinarySettings Class
			services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
			services.AddAutoMapper(typeof(DatingRepository).Assembly);
			services.AddScoped<IAuthRepository, AuthRepository>();
			services.AddScoped<IDatingRepository, DatingRepository>();
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						// validates token secret
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.ASCII.GetBytes(
								Configuration.GetSection("AppSettings:Token").Value
							)
						),
						ValidateIssuer = false,
						ValidateAudience = false
					};
				});

			// cookie setting for indentity user
			// services.ConfigureApplicationCookie(options =>
			// {
			// 	options.Cookie.Name = "user";
			// 	options.Cookie.Expiration = TimeSpan.FromDays(1);
			// });

			services.AddCors(options =>
			{
				options.AddPolicy("CORSPolicy",
							// with orgins is the base of our client app
							builder => builder.WithOrigins("http://localhost:4200")
							.AllowAnyMethod()
							.AllowAnyHeader()
							.AllowCredentials() // use this for cookies
						);
			});
			// Adding our action filter
			services.AddScoped<LogUserActivity>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		// Orering of middlewares is important here
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				//app.UseHsts();
				app.UseExceptionHandler(builder =>
				{
					builder.Run(async context =>
					{
						context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

						var error = context.Features.Get<IExceptionHandlerFeature>();

						if (error != null)
						{
							// writes error message in response
							context.Response.AddApplicationError(error.Error.Message);
							await context.Response.WriteAsync(error.Error.Message);
						}
					});
				});
			}

			app.UseCors("CORSPolicy");
			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseMvc();
		}
	}
}
