using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Phrook.Customizations.Identity;
using Phrook.Models.Entities;
using Phrook.Models.Options;
using Phrook.Models.Services.Application;
using Phrook.Models.Services.HttpClients;
using Phrook.Models.Services.Infrastructure;

namespace Phrook
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		// private readonly IOptionsMonitor<ConnectionStringsOptions> connectionStringOptions;
		public Startup(IConfiguration configuration/*, IOptionsMonitor<ConnectionStringsOptions> connectionStringOptions */)
		{
			//this.connectionStringOptions = connectionStringOptions;
			this.Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRazorPages();
			services.AddMvc();
			services.AddTransient<IBookService, EfCoreBookService>();
			services.AddHttpClient<IGoogleBooksClient, GoogleBooksClient>();
			services.AddDbContextPool<PhrookDbContext>(optionsBuilder =>
			{
				string connectionString = Configuration["ConnectionStrings:Default"];
				optionsBuilder.UseSqlite(connectionString);
			});

			services.AddDefaultIdentity<ApplicationUser>(options => {
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 8;
				options.Password.RequiredUniqueChars = 4;
				options.Password.RequireUppercase = true;
				options.Password.RequireLowercase = true;				
				options.Password.RequireNonAlphanumeric = true;
			})
			.AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
			.AddPasswordValidator<CommonPasswordValidator<ApplicationUser>>()
			.AddEntityFrameworkStores<PhrookDbContext>();

			#region OPTIONS
			services.Configure<ConnectionStringsOptions>(Configuration.GetSection("ConnectionStrings"));
			services.Configure<BooksOptions>(Configuration.GetSection("Books"));
			services.Configure<BooksOrderOptions>(Configuration.GetSection("Books:Order"));
			services.Configure<GoogleBooksApiOptions>(Configuration.GetSection("GoogleBooksApi"));
			#endregion
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();

				//Updating a file for triggering BrowserSync and reload the page
				lifetime.ApplicationStarted.Register(() =>
				{
					string filePath = Path.Combine(env.ContentRootPath, "bin/reload.txt");
					File.WriteAllText(filePath, DateTime.Now.ToString());
				});
			}
			else
			{
				app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandlingPath = "/Error",
                    AllowStatusCode404Response = true
                });
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			var appCulture = CultureInfo.InvariantCulture;
			app.UseRequestLocalization(new RequestLocalizationOptions 
			{
				DefaultRequestCulture = new RequestCulture(appCulture),
				SupportedCultures = new[] {appCulture}
			});

			app.UseRouting();
			
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				// redirection starts
				// endpoints.MapGet("/", async context =>
				// {
				//     context.Response.Redirect("/Books/Index");
				// });
				//redirection ends
				endpoints.MapControllerRoute("default", "{controller=Books}/{action=Index}/{id?}");
				endpoints.MapRazorPages();
			});
		}
	}
}
