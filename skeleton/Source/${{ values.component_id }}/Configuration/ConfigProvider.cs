using Elements.ConfigServer.Client;
using Elements.ConfigServer.Client.ConfigurationManager;
using Elements.ConfigServer.Client.Entities;
using Elements.Logging;
using Elements.Logging.Tracing;
using Elements.LoggingContract;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ${{ values.component_id }}.Configuration
{
	public class ConfigProvider : IConfigProvider
	{
		private static readonly IConfigManager<ConfigWrapper> _configManager;
		private static readonly IConfigurationRoot _appsettings;

		static ConfigProvider()
		{
			_appsettings = new ConfigurationBuilder()
					.AddJsonFile("appsettings.json", false, true)
					.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
					.Build();

			_configManager = ConfigManagerFactory<ConfigWrapper>.CreateConfigManager(
					key => _appsettings[$"Appsettings:{key}"],
					Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"),
					new List<SchemaEntry>
					{
						//SchemaEntry.Create<SomeSchema>() //TODO: Add schemas here
					},
					shouldSendSchemas: true,
					headersToUpdate: new Dictionary<string, Func<string>>
					{
						{ LoggingMiddleware.RelatedModulesHeaderName, () => Constants.ServiceName }
					},
					onCreationFailed: ex =>
						Tracer.TraceErrorMessage(ex, "Failed to get configuration from ConfigServer"),
					enableCaching: true);
		}

		public static bool EnableHttpServer
		{
			get
			{
				var httpServerStr = _appsettings["Appsettings:EnableHttpServer"];
				return !string.IsNullOrWhiteSpace(httpServerStr)
					&& bool.TryParse(httpServerStr, out var isHttpServer)
					&& isHttpServer;
			}
		}

		public async Task SubscribeAsync(Action<Config<ConfigWrapper>> onConfigChanged, Action<Exception> onError, CancellationToken cancellationToken)
		{
			using var tracer = Tracer.TraceMethod(LogLevel.ExternalCall, $"{nameof(ConfigProvider)}.{nameof(SubscribeAsync)}");
			await Task.Run(() => _configManager.Subscribe(onConfigChanged, onError, cancellationToken), cancellationToken);
		}

		public async Task<ConfigWrapper> GetConfigurationAsync(string tenantId = null)
		{
			using var tracer = Tracer.TraceMethod(LogLevel.ExternalCall, $"{nameof(ConfigProvider)}.{nameof(GetConfigurationAsync)}", () => new Dictionary<string, object>
			{
				["tenantId"] = tenantId
			});

			return string.IsNullOrWhiteSpace(tenantId)
				? await _configManager.GetGlobalConfigAsync()
				: await _configManager.GetTenantConfigAsync(tenantId);
		}
	}
}
