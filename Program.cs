using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Phrook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, builder) => {
					//command line arguments cannot be used
					builder.Sources.RemoveAt(builder.Sources.Count-1);
					//environment variables cannot be used
					builder.Sources.RemoveAt(builder.Sources.Count-1);
					var env = context.HostingEnvironment;
					if(!env.IsDevelopment())
					{
						var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
						if(appAssembly != null)
						{
							builder.AddUserSecrets(appAssembly, optional: true);
						}
					}

					//if you need environment variables or command line arguments as configurations
					// builder.AddEnvironmentVariables();
					// builder.AddCommandLine();
				})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
