using Elements.Logging.Tracing;
using Elements.LoggingContract;
using ${{ values.component_id }}.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ${{ values.component_id }}
{
	public class Worker : BackgroundService
	{
		private readonly IHostApplicationLifetime _hostApplicationLifetime;
		private readonly IConfigProvider _configProvider;

		public Worker(
			IHostApplicationLifetime hostApplicationLifetime,
			IConfigProvider configProvider)
		{
			_hostApplicationLifetime = hostApplicationLifetime;
			_configProvider = configProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			using var tracer = Tracer.TraceMethod(LogLevel.Verbose, $"{nameof(Worker)}.{nameof(ExecuteAsync)}");

			try
			{
				tracer.TraceInfo($"Worker running", LogLevel.Minimum);
				await MultiTenantService.StartAsync(_configProvider, stoppingToken);
			}
			catch (Exception e)
			{
				tracer.TraceError(e, "Worker crashed");
				_hostApplicationLifetime.StopApplication();
			}
		}
	}
}
