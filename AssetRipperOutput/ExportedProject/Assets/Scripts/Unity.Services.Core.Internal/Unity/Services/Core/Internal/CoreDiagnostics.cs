using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core.Telemetry.Internal;

namespace Unity.Services.Core.Internal
{
	internal class CoreDiagnostics
	{
		internal const string CorePackageName = "com.unity.services.core";

		internal const string CircularDependencyDiagnosticName = "circular_dependency";

		internal const string CorePackageInitDiagnosticName = "core_package_init";

		internal const string OperateServicesInitDiagnosticName = "operate_services_init";

		internal const string ProjectConfigTagName = "project_config";

		public static CoreDiagnostics Instance { get; internal set; }

		public IDictionary<string, string> CoreTags { get; } = new Dictionary<string, string>();


		internal IDiagnosticsComponentProvider DiagnosticsComponentProvider { get; set; }

		internal IDiagnostics Diagnostics { get; set; }

		public void SetProjectConfiguration(string serializedProjectConfig)
		{
			CoreTags["project_config"] = serializedProjectConfig;
		}

		public void SendCircularDependencyDiagnostics(Exception exception)
		{
			SendCoreDiagnosticsAsync("circular_dependency", exception).ContinueWith(OnSendFailed, TaskContinuationOptions.OnlyOnFaulted);
		}

		public void SendCorePackageInitDiagnostics(Exception exception)
		{
			SendCoreDiagnosticsAsync("core_package_init", exception).ContinueWith(OnSendFailed, TaskContinuationOptions.OnlyOnFaulted);
		}

		public void SendOperateServicesInitDiagnostics(Exception exception)
		{
			SendCoreDiagnosticsAsync("operate_services_init", exception).ContinueWith(OnSendFailed, TaskContinuationOptions.OnlyOnFaulted);
		}

		internal async Task SendCoreDiagnosticsAsync(string diagnosticName, Exception exception)
		{
			(await GetOrCreateDiagnosticsAsync())?.SendDiagnostic(diagnosticName, exception?.ToString(), CoreTags);
		}

		private static void OnSendFailed(Task failedSendTask)
		{
			CoreLogger.LogException(failedSendTask.Exception);
		}

		internal async Task<IDiagnostics> GetOrCreateDiagnosticsAsync()
		{
			if (Diagnostics != null)
			{
				return Diagnostics;
			}
			if (DiagnosticsComponentProvider == null)
			{
				return null;
			}
			Diagnostics = (await DiagnosticsComponentProvider.CreateDiagnosticsComponents()).Create("com.unity.services.core");
			SetProjectConfiguration(await DiagnosticsComponentProvider.GetSerializedProjectConfigurationAsync());
			return Diagnostics;
		}
	}
}
