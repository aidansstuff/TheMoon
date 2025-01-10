using System.Collections.Generic;

namespace Unity.Services.Qos
{
	internal class Configuration
	{
		public string BasePath;

		public int? RequestTimeout;

		public int? NumberOfRetries;

		public IDictionary<string, string> Headers;

		public Configuration(string basePath, int? requestTimeout, int? numRetries, IDictionary<string, string> headers)
		{
			BasePath = basePath;
			RequestTimeout = requestTimeout;
			NumberOfRetries = numRetries;
			if (headers == null)
			{
				Headers = headers;
			}
			else
			{
				Headers = new Dictionary<string, string>(headers);
			}
		}

		public static Configuration MergeConfigurations(Configuration a, Configuration b)
		{
			if (a == null || b == null)
			{
				return a ?? b;
			}
			Configuration configuration = new Configuration(a.BasePath, a.RequestTimeout, a.NumberOfRetries, a.Headers);
			if (configuration.BasePath == null)
			{
				configuration.BasePath = b.BasePath;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (b.Headers != null)
			{
				foreach (KeyValuePair<string, string> header in b.Headers)
				{
					dictionary[header.Key] = header.Value;
				}
			}
			if (configuration.Headers != null)
			{
				foreach (KeyValuePair<string, string> header2 in configuration.Headers)
				{
					dictionary[header2.Key] = header2.Value;
				}
			}
			configuration.Headers = dictionary;
			configuration.RequestTimeout = configuration.RequestTimeout ?? b.RequestTimeout;
			configuration.NumberOfRetries = configuration.NumberOfRetries ?? b.NumberOfRetries;
			return configuration;
		}
	}
}
