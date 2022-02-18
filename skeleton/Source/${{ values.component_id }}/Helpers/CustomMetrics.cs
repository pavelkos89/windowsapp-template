using Prometheus;

namespace ${{ values.component_id }}.Helpers
{
	public static class CustomMetrics
	{
		public static Counter SomeCounter;

		public static void Init()
		{
			// TODO: Implement your counters. If you don't have any counters, remove this class and all references to it
			SomeCounter = Metrics.CreateCounter("service_some_counter", "Some counter");
		}
	}
}
