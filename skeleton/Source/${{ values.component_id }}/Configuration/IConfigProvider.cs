using Elements.ConfigServer.Client.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ${{ values.component_id }}.Configuration
{
	public interface IConfigProvider
	{
		Task SubscribeAsync(Action<Config<ConfigWrapper>> onConfigChanged, Action<Exception> onError, CancellationToken cancellationToken);
		Task<ConfigWrapper> GetConfigurationAsync(string tenantId = null);
	}
}
