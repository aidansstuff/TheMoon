using System;
using System.Collections.Generic;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class MetricsFactory : IMetricsFactory, IServiceComponent
	{
		private readonly IProjectConfiguration m_ProjectConfig;

		public IReadOnlyDictionary<string, string> CommonTags { get; }

		internal MetricsHandler Handler { get; }

		public MetricsFactory(MetricsHandler handler, IProjectConfiguration projectConfig)
		{
			Handler = handler;
			m_ProjectConfig = projectConfig;
			CommonTags = new Dictionary<string, string>(handler.Cache.Payload.CommonTags).MergeAllowOverride(handler.Cache.Payload.MetricsCommonTags);
		}

		public IMetrics Create(string packageName)
		{
			if (string.IsNullOrEmpty(packageName))
			{
				throw new ArgumentNullException("packageName");
			}
			IDictionary<string, string> packageTags = FactoryUtils.CreatePackageTags(m_ProjectConfig, packageName);
			return new Metrics(Handler, packageTags);
		}
	}
}
