using System;
using System.Collections.Generic;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Telemetry.Internal;

namespace Unity.Services.Core.Internal
{
	internal class CoreMetrics
	{
		internal const string PackageInitTimeMetricName = "package_init_time";

		internal const string AllPackagesInitSuccessMetricName = "all_packages_init_success";

		internal const string AllPackagesInitTimeMetricName = "all_packages_init_time";

		internal const string PackageInitializerNamesKeyFormat = "{0}.initializer-assembly-qualified-names";

		internal const char PackageInitializerNamesSeparator = ';';

		internal const string AllPackageNamesKey = "com.unity.services.core.all-package-names";

		internal const char AllPackageNamesSeparator = ';';

		public static CoreMetrics Instance { get; internal set; }

		internal IMetrics Metrics { get; set; }

		internal IDictionary<Type, IMetrics> AllPackageMetrics { get; } = new Dictionary<Type, IMetrics>();


		public void SendAllPackagesInitSuccessMetric()
		{
			if (Metrics != null)
			{
				Metrics.SendSumMetric("all_packages_init_success");
			}
		}

		public void SendAllPackagesInitTimeMetric(double initTimeSeconds)
		{
			if (Metrics != null)
			{
				Metrics.SendHistogramMetric("all_packages_init_time", initTimeSeconds);
			}
		}

		public void SendInitTimeMetricForPackage(Type packageType, double initTimeSeconds)
		{
			if (AllPackageMetrics.TryGetValue(packageType, out var value))
			{
				value.SendHistogramMetric("package_init_time", initTimeSeconds);
			}
		}

		public void Initialize(IProjectConfiguration configuration, IMetricsFactory factory, Type corePackageType)
		{
			AllPackageMetrics.Clear();
			FindAndCacheAllPackageMetrics(configuration, factory);
			if (AllPackageMetrics.TryGetValue(corePackageType, out var value))
			{
				Metrics = value;
			}
		}

		internal void FindAndCacheAllPackageMetrics(IProjectConfiguration configuration, IMetricsFactory factory)
		{
			string[] array = configuration.GetString("com.unity.services.core.all-package-names", "")?.Split(';') ?? Array.Empty<string>();
			foreach (string text in array)
			{
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				string key = $"{text}.initializer-assembly-qualified-names";
				string @string = configuration.GetString(key, "");
				if (string.IsNullOrEmpty(@string))
				{
					continue;
				}
				string[] array2 = @string.Split(';');
				for (int j = 0; j < array2.Length; j++)
				{
					Type type = Type.GetType(array2[j]);
					if ((object)type != null)
					{
						IMetrics value = factory.Create(text);
						AllPackageMetrics[type] = value;
					}
				}
			}
		}
	}
}
