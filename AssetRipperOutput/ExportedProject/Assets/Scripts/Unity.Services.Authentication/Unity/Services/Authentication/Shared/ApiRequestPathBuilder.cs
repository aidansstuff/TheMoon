using System;
using System.Collections.Generic;

namespace Unity.Services.Authentication.Shared
{
	internal class ApiRequestPathBuilder
	{
		private string _baseUrl;

		private string _path;

		private string _query = "?";

		public ApiRequestPathBuilder(string baseUrl, string path)
		{
			_baseUrl = baseUrl;
			_path = path;
		}

		public void AddPathParameters(Dictionary<string, string> parameters)
		{
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				_path = _path.Replace("{" + parameter.Key + "}", Uri.EscapeDataString(parameter.Value));
			}
		}

		public void AddQueryParameters(Multimap<string, string> parameters)
		{
			foreach (KeyValuePair<string, IList<string>> parameter in parameters)
			{
				foreach (string item in parameter.Value)
				{
					_query = _query + parameter.Key + "=" + Uri.EscapeDataString(item) + "&";
				}
			}
		}

		public string GetFullUri()
		{
			return _baseUrl + _path + _query.Substring(0, _query.Length - 1);
		}
	}
}
