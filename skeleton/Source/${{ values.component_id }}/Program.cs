using Elements.Logging;
using Elements.Logging.Adapters.Microsoft;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ${{ values.component_id }}.Configuration;
using ${{ values.component_id }}.Helpers;


namespace ${{ values.component_id }}
{
	public class Program
	{
		public static void Main(string[] args)
		{
			TracerHelper.InitializeLogging();

			if (ConfigProvider.EnableHttpServer)
			{
				CreateWebHostBuilder(args).Build().Run();
			}
			else
			{
				CreateHostBuilder(args).Build().Run();
			}
		}

		public static IHostBuilder CreateWebHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseWindowsService()
				.ConfigureLogging((hostingContext, logging) => ConfigureLogging(logging))
				.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseWindowsService()
				.ConfigureLogging((hostingContext, logging) => ConfigureLogging(logging))
				.ConfigureServices((hostContext, services) => services.ConfigureServices());

		private static void ConfigureLogging(ILoggingBuilder logging)
		{
			logging.Services.AddTransient<Elements.LoggingContract.ILogger, Logger>();
			logging.AddDefaultElementsLogger();
		}
	}
}
