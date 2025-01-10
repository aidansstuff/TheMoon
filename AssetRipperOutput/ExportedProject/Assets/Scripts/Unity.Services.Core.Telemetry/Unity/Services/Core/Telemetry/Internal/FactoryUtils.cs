using System.Collections.Generic;
using Unity.Services.Core.Configuration.Internal;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal static class FactoryUtils
	{
		internal const string PackageVersionKeyFormat = "{0}.version";

		public static IDictionary<string, string> CreatePackageTags(IProjectConfiguration projectConfig, string packageName)
		{
			string @string = projectConfig.GetString($"{packageName}.version", string.Empty);
			string.IsNullOrEmpty(@string);
			return new Dictionary<string, string>
			{
				["package_name"] = packageName,
				["package_version"] = @string
			};
		}
	}
}
