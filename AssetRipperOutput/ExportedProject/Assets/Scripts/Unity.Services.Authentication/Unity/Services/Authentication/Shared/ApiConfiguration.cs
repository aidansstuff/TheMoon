using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Unity.Services.Authentication.Shared
{
	internal class ApiConfiguration : IApiConfiguration
	{
		public const string ISO8601_DATETIME_FORMAT = "o";

		private string _basePath;

		private IDictionary<string, string> _apiKey;

		private IDictionary<string, string> _apiKeyPrefix;

		private string _dateTimeFormat = "o";

		private string _tempFolderPath = Path.GetTempPath();

		public virtual string BasePath
		{
			get
			{
				return _basePath;
			}
			set
			{
				_basePath = value;
			}
		}

		public virtual IDictionary<string, string> DefaultHeaders { get; set; }

		public virtual int Timeout { get; set; }

		public virtual string UserAgent { get; set; }

		public virtual string Username { get; set; }

		public virtual string Password { get; set; }

		public virtual string AccessToken { get; set; }

		public virtual string DateTimeFormat
		{
			get
			{
				return _dateTimeFormat;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_dateTimeFormat = "o";
				}
				else
				{
					_dateTimeFormat = value;
				}
			}
		}

		public virtual IDictionary<string, string> ApiKeyPrefix
		{
			get
			{
				return _apiKeyPrefix;
			}
			set
			{
				if (value == null)
				{
					throw new InvalidOperationException("ApiKeyPrefix collection may not be null.");
				}
				_apiKeyPrefix = value;
			}
		}

		public virtual IDictionary<string, string> ApiKey
		{
			get
			{
				return _apiKey;
			}
			set
			{
				if (value == null)
				{
					throw new InvalidOperationException("ApiKey collection may not be null.");
				}
				_apiKey = value;
			}
		}

		public ApiConfiguration()
		{
			UserAgent = WebUtility.UrlEncode("openapi-generator/csharp");
			DefaultHeaders = new ConcurrentDictionary<string, string>();
			ApiKey = new ConcurrentDictionary<string, string>();
			ApiKeyPrefix = new ConcurrentDictionary<string, string>();
			Timeout = 10;
		}

		public ApiConfiguration(IDictionary<string, string> defaultHeaders, IDictionary<string, string> apiKey, IDictionary<string, string> apiKeyPrefix, string basePath)
			: this()
		{
			if (string.IsNullOrWhiteSpace(basePath))
			{
				throw new ArgumentException("The provided basePath is invalid.", "basePath");
			}
			if (defaultHeaders == null)
			{
				throw new ArgumentNullException("defaultHeaders");
			}
			if (apiKey == null)
			{
				throw new ArgumentNullException("apiKey");
			}
			if (apiKeyPrefix == null)
			{
				throw new ArgumentNullException("apiKeyPrefix");
			}
			BasePath = basePath;
			foreach (KeyValuePair<string, string> defaultHeader in defaultHeaders)
			{
				DefaultHeaders.Add(defaultHeader);
			}
			foreach (KeyValuePair<string, string> item in apiKey)
			{
				ApiKey.Add(item);
			}
			foreach (KeyValuePair<string, string> item2 in apiKeyPrefix)
			{
				ApiKeyPrefix.Add(item2);
			}
		}

		public string GetApiKeyWithPrefix(string apiKeyIdentifier)
		{
			ApiKey.TryGetValue(apiKeyIdentifier, out var value);
			if (ApiKeyPrefix.TryGetValue(apiKeyIdentifier, out var value2))
			{
				return value2 + " " + value;
			}
			return value;
		}

		public void AddApiKey(string key, string value)
		{
			ApiKey[key] = value;
		}

		public void AddApiKeyPrefix(string key, string value)
		{
			ApiKeyPrefix[key] = value;
		}
	}
}
