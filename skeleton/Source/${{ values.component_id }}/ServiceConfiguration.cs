using Microsoft.Extensions.DependencyInjection;
using ${{ values.component_id }}.Configuration;

namespace ${{ values.component_id }}
{
	public static class ServiceConfiguration
	{
		public static void ConfigureServices(this IServiceCollection services)
		{
			// TODO: Register all required services here

			services.AddSingleton<IConfigProvider, ConfigProvider>();
			services.AddHostedService<Worker>();
		}
	}
}
