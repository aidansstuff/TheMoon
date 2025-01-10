using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Unity.Services.Core.Configuration;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Device;
using Unity.Services.Core.Device.Internal;
using Unity.Services.Core.Environments.Internal;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Internal.Serialization;
using Unity.Services.Core.Scheduler.Internal;
using Unity.Services.Core.Telemetry.Internal;
using Unity.Services.Core.Threading.Internal;
using UnityEngine;

namespace Unity.Services.Core.Registration
{
	internal class CorePackageInitializer : IInitializablePackage, IDiagnosticsComponentProvider
	{
		internal const string CorePackageName = "com.unity.services.core";

		internal const string ProjectUnlinkMessage = "To use Unity's dashboard services, you need to link your Unity project to a project ID. To do this, go to Project Settings to select your organization, select your project and then link a project ID. You also need to make sure your organization has access to the required products. Visit https://dashboard.unity3d.com to sign up.";

		private readonly IJsonSerializer m_Serializer;

		private InitializationOptions m_CurrentInitializationOptions;

		internal ActionScheduler ActionScheduler { get; private set; }

		internal InstallationId InstallationId { get; private set; }

		internal ProjectConfiguration ProjectConfig { get; private set; }

		internal Unity.Services.Core.Environments.Internal.Environments Environments { get; private set; }

		internal ExternalUserId ExternalUserId { get; private set; }

		internal ICloudProjectId CloudProjectId { get; private set; }

		internal IDiagnosticsFactory DiagnosticsFactory { get; private set; }

		internal IMetricsFactory MetricsFactory { get; private set; }

		internal UnityThreadUtilsInternal UnityThreadUtils { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Register()
		{
			CorePackageInitializer corePackageInitializer = new CorePackageInitializer(new NewtonsoftSerializer());
			CoreDiagnostics.Instance.DiagnosticsComponentProvider = corePackageInitializer;
			CoreRegistry.Instance.RegisterPackage(corePackageInitializer).ProvidesComponent<IInstallationId>().ProvidesComponent<ICloudProjectId>()
				.ProvidesComponent<IActionScheduler>()
				.ProvidesComponent<IEnvironments>()
				.ProvidesComponent<IProjectConfiguration>()
				.ProvidesComponent<IMetricsFactory>()
				.ProvidesComponent<IDiagnosticsFactory>()
				.ProvidesComponent<IUnityThreadUtils>()
				.ProvidesComponent<IExternalUserId>();
		}

		public CorePackageInitializer(IJsonSerializer serializer)
		{
			m_Serializer = serializer;
		}

		public async Task Initialize(CoreRegistry registry)
		{
			try
			{
				if (HaveInitOptionsChanged())
				{
					FreeOptionsDependantComponents();
				}
				InitializeInstallationId();
				InitializeActionScheduler();
				await InitializeProjectConfigAsync(UnityServices.Instance.Options);
				InitializeExternalUserId(ProjectConfig);
				InitializeEnvironments(ProjectConfig);
				InitializeCloudProjectId();
				if (string.IsNullOrEmpty(CloudProjectId.GetCloudProjectId()))
				{
					throw new UnityProjectNotLinkedException("To use Unity's dashboard services, you need to link your Unity project to a project ID. To do this, go to Project Settings to select your organization, select your project and then link a project ID. You also need to make sure your organization has access to the required products. Visit https://dashboard.unity3d.com to sign up.");
				}
				InitializeDiagnostics(ActionScheduler, ProjectConfig, CloudProjectId, Environments);
				CoreDiagnostics.Instance.Diagnostics = DiagnosticsFactory.Create("com.unity.services.core");
				CoreDiagnostics.Instance.SetProjectConfiguration(ProjectConfig.ToJson());
				InitializeMetrics(ActionScheduler, ProjectConfig, CloudProjectId, Environments);
				CoreMetrics.Instance.Initialize(ProjectConfig, MetricsFactory, GetType());
				InitializeUnityThreadUtils();
				RegisterProvidedComponents();
			}
			catch (Exception reason2) when (SendFailedInitDiagnostic(reason2))
			{
			}
			void RegisterProvidedComponents()
			{
				registry.RegisterServiceComponent((IInstallationId)InstallationId);
				registry.RegisterServiceComponent((IActionScheduler)ActionScheduler);
				registry.RegisterServiceComponent((IProjectConfiguration)ProjectConfig);
				registry.RegisterServiceComponent((IEnvironments)Environments);
				registry.RegisterServiceComponent(CloudProjectId);
				registry.RegisterServiceComponent(DiagnosticsFactory);
				registry.RegisterServiceComponent(MetricsFactory);
				registry.RegisterServiceComponent((IUnityThreadUtils)UnityThreadUtils);
				registry.RegisterServiceComponent((IExternalUserId)ExternalUserId);
			}
			static bool SendFailedInitDiagnostic(Exception reason)
			{
				CoreDiagnostics.Instance.SendCorePackageInitDiagnostics(reason);
				return false;
			}
		}

		private bool HaveInitOptionsChanged()
		{
			if (m_CurrentInitializationOptions != null)
			{
				return !m_CurrentInitializationOptions.Values.ValueEquals(UnityServices.Instance.Options.Values);
			}
			return false;
		}

		private void FreeOptionsDependantComponents()
		{
			ProjectConfig = null;
			Environments = null;
			DiagnosticsFactory = null;
			MetricsFactory = null;
		}

		internal void InitializeInstallationId()
		{
			if (InstallationId == null)
			{
				InstallationId installationId = new InstallationId();
				installationId.CreateIdentifier();
				InstallationId = installationId;
			}
		}

		internal void InitializeActionScheduler()
		{
			if (ActionScheduler == null)
			{
				ActionScheduler actionScheduler = new ActionScheduler();
				actionScheduler.JoinPlayerLoopSystem();
				ActionScheduler = actionScheduler;
			}
		}

		internal async Task InitializeProjectConfigAsync([NotNull] InitializationOptions options)
		{
			if (ProjectConfig == null)
			{
				ProjectConfig = await GenerateProjectConfigurationAsync(options);
				m_CurrentInitializationOptions = new InitializationOptions(options);
			}
		}

		internal async Task<ProjectConfiguration> GenerateProjectConfigurationAsync([NotNull] InitializationOptions options)
		{
			SerializableProjectConfiguration config = await GetSerializedConfigOrEmptyAsync();
			if (config.Keys == null || config.Values == null)
			{
				config = SerializableProjectConfiguration.Empty;
			}
			Dictionary<string, ConfigurationEntry> dictionary = new Dictionary<string, ConfigurationEntry>(config.Keys.Length);
			dictionary.FillWith(config);
			dictionary.FillWith(options);
			return new ProjectConfiguration(dictionary, m_Serializer);
		}

		internal static async Task<SerializableProjectConfiguration> GetSerializedConfigOrEmptyAsync()
		{
			try
			{
				return await ConfigurationUtils.ConfigurationLoader.GetConfigAsync();
			}
			catch (Exception ex)
			{
				CoreLogger.LogError("An error occured while trying to get the project configuration for services.\n" + ex.Message + "\n" + ex.StackTrace);
				return SerializableProjectConfiguration.Empty;
			}
		}

		internal void InitializeExternalUserId(IProjectConfiguration projectConfiguration)
		{
			if (UnityServices.ExternalUserId == null)
			{
				string @string = projectConfiguration.GetString("com.unity.services.core.analytics-user-id");
				if (!string.IsNullOrEmpty(@string))
				{
					UnityServices.ExternalUserId = @string;
				}
			}
			if (ExternalUserId == null)
			{
				ExternalUserId = new ExternalUserId();
			}
		}

		internal void InitializeEnvironments(IProjectConfiguration projectConfiguration)
		{
			if (Environments == null)
			{
				string @string = projectConfiguration.GetString("com.unity.services.core.environment-name", "production");
				Environments = new Unity.Services.Core.Environments.Internal.Environments
				{
					Current = @string
				};
			}
		}

		internal void InitializeCloudProjectId(ICloudProjectId cloudProjectId = null)
		{
			if (CloudProjectId == null)
			{
				CloudProjectId = cloudProjectId ?? new CloudProjectId();
			}
		}

		internal void InitializeDiagnostics(IActionScheduler scheduler, IProjectConfiguration projectConfiguration, ICloudProjectId cloudProjectId, IEnvironments environments)
		{
			if (DiagnosticsFactory == null)
			{
				DiagnosticsFactory = TelemetryUtils.CreateDiagnosticsFactory(scheduler, projectConfiguration, cloudProjectId, environments);
			}
		}

		internal void InitializeMetrics(IActionScheduler scheduler, IProjectConfiguration projectConfiguration, ICloudProjectId cloudProjectId, IEnvironments environments)
		{
			if (MetricsFactory == null)
			{
				MetricsFactory = TelemetryUtils.CreateMetricsFactory(scheduler, projectConfiguration, cloudProjectId, environments);
			}
		}

		internal void InitializeUnityThreadUtils()
		{
			if (UnityThreadUtils == null)
			{
				UnityThreadUtils = new UnityThreadUtilsInternal();
			}
		}

		public async Task<IDiagnosticsFactory> CreateDiagnosticsComponents()
		{
			if (HaveInitOptionsChanged())
			{
				FreeOptionsDependantComponents();
			}
			InitializeActionScheduler();
			await InitializeProjectConfigAsync(UnityServices.Instance.Options);
			InitializeEnvironments(ProjectConfig);
			InitializeCloudProjectId();
			InitializeDiagnostics(ActionScheduler, ProjectConfig, CloudProjectId, Environments);
			return DiagnosticsFactory;
		}

		[Conditional("ENABLE_UNITY_SERVICES_CORE_VERBOSE_LOGGING")]
		private void LogInitializationInfoJson()
		{
			JObject jObject = new JObject();
			JObject jObject2 = JObject.Parse(m_Serializer.SerializeObject(DiagnosticsFactory.CommonTags));
			JObject value = JObject.Parse(ProjectConfig.ToJson());
			JObject content = JObject.Parse("{\"installation_id\": \"" + InstallationId.Identifier + "\"}");
			jObject2.Merge(content);
			jObject.Add("CommonSettings", jObject2);
			jObject.Add("ServicesRuntimeSettings", value);
		}

		public async Task<string> GetSerializedProjectConfigurationAsync()
		{
			await InitializeProjectConfigAsync(UnityServices.Instance.Options);
			return ProjectConfig.ToJson();
		}
	}
}
