using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ${{ values.component_id }}.Helpers;
using ${{ values.component_id }}.Web;
using Prometheus;
using System.Reflection;
using System.Text.Json;

namespace ${{ values.component_id }}
{
	public class Startup
	{
		private static readonly VersionInfo _versionInfo = new(Assembly.GetExecutingAssembly());

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.ConfigureServices();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			// TODO: Init custom Prometheus metrics if necessary
			CustomMetrics.Init();

			// Prometheus middleware. Gives /metrics endpoint.
			app.UseMetricServer();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync($"${{ values.component_id }} ({_versionInfo.InformationalVersion})");
				});

				endpoints.MapGet("/api/version", async context =>
				{
					context.Response.ContentType = "application/json; charset=utf-8";
					await context.Response.WriteAsync(JsonSerializer.Serialize(_versionInfo));
				});
			});
		}
	}
}
