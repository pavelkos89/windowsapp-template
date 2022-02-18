using Elements.Logging.Tracing;
using Elements.LoggingContract;
using System;

namespace ${{ values.component_id }}.Helpers
{
	public static class TracerHelper
	{
		public static void InitializeLogging()
		{
			Tracer.EnableLogging(Constants.ServiceName);

			AppDomain.CurrentDomain.UnhandledException += (o, arg) =>
			{
				var errorMessage = $"Unhandled exception occured in '{AppDomain.CurrentDomain.FriendlyName}'{Environment.NewLine}";
				errorMessage += arg.IsTerminating ? "Terminate application" : "Application will not terminate";

				Tracer.TraceErrorMessage((Exception)arg.ExceptionObject, errorMessage);
			};
		}

		public static void StartLoggingOperation(string tenantId, LogLevel logLevel, string correlationId = null)
		{
			if (string.IsNullOrEmpty(correlationId))
				correlationId = Guid.NewGuid().ToString();

			Tracer.StartLoggingOperation(new ElementsLogContext(tenantId, logLevel, Constants.ServiceName, correlationId));
		}
	}
}
