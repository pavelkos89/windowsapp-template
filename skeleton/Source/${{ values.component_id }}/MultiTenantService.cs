using Elements.ConfigServer.Client.Entities;
using Elements.Logging.Tracing;
using Elements.LoggingContract;
using ${{ values.component_id }}.Configuration;
using ${{ values.component_id }}.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ${{ values.component_id }}
{
	public static class MultiTenantService
	{
		private static readonly SemaphoreSlim _asyncLock = new(1);

		public static async Task StartAsync(
			IConfigProvider configProvider,
			CancellationToken cancellationToken)
		{
			using var multiTenantTracer = Tracer.TraceMethod(LogLevel.Minimum, $"{nameof(MultiTenantService)}.{nameof(StartAsync)}");

			CancellationTokenSource cts = null;
			try
			{
				// Subscribing to configuration changes. Whenever a change happens we tear down all existing services and re-initialize.
				await configProvider
					.SubscribeAsync(config =>
					{
						using var tracer = Tracer.TraceMethod(LogLevel.Operation, "ConfigManager.Subscribe");

						if (cts != null)
						{
							cts.Cancel();
							cts.Dispose();
							cts = null;
							tracer.TraceInfo("Restarting the service due to a configuration change", LogLevel.Minimum);
						}

						cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
						using var registration = cancellationToken.Register(() => { cts?.Cancel(); cts?.Dispose(); cts = null; });
						Run(config, cts.Token);
					}, x => Tracer.TraceErrorMessage(x), cancellationToken);
			}
			catch (Exception ex)
			{
				Tracer.TraceErrorMessage(ex, "Failed to start service.");
				throw;
			}
		}

		private static void Run(
			Config<ConfigWrapper> config,
			CancellationToken cancellationToken)
		{
			TracerHelper.StartLoggingOperation("No tenant", LogLevel.MainOperation);

			Task.Run(async () =>
			{
				using var tracer = Tracer.TraceMethod(LogLevel.MainOperation, $"{nameof(MultiTenantService)}.{nameof(Run)}");

				try
				{
					await _asyncLock.WaitAsync();

					var enabledConfigs = config.ValidatedTenantConfigs
						//.Where(x => x.IsValid && x.Config.YourComponent.Enabled) // TODO
						.Select(x => x.Config)
						.ToList();
					if (!enabledConfigs.Any())
					{
						tracer.TraceInfo($"Service '{Constants.ServiceName}' is not enabled for any tenant");
						return;
					}

					tracer.TraceInfo($"MultiTenantService.Run is processing {enabledConfigs.Count} valid tenant(s)", LogLevel.MainOperation);

					// TODO: Implement
					var processingTasks = new List<Task>();
					await Task.WhenAll(processingTasks);
				}
				catch (OperationCanceledException e)
				{
					tracer.TraceError(e, "Task was cancelled");
				}
				catch (Exception e)
				{
					tracer.TraceError(e, "Service failed to start");
				}
				finally
				{
					_asyncLock.Release();
				}
			}, cancellationToken);
		}
	}
}
