using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Services.Authentication
{
	internal static class HttpUtilities
	{
		public static IDictionary<string, string> ParseQueryString(string queryString)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = queryString.Split('?', '&');
			foreach (string text in array)
			{
				int num = text.IndexOf('=');
				if (num >= 0)
				{
					string key = UnescapeUrlString(text.Substring(0, num));
					string value = UnescapeUrlString(text.Substring(num + 1));
					dictionary[key] = value;
				}
			}
			return dictionary;
		}

		public static string EncodeQueryString(IDictionary<string, string> queryParams)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (KeyValuePair<string, string> queryParam in queryParams)
			{
				if (!flag)
				{
					stringBuilder.Append('&');
				}
				else
				{
					flag = false;
				}
				stringBuilder.Append(EscapeUrlString(queryParam.Key)).Append('=').Append(EscapeUrlString(queryParam.Value));
			}
			return stringBuilder.ToString();
		}

		private static string EscapeUrlString(string rawString)
		{
			return Uri.EscapeDataString(rawString);
		}

		private static string UnescapeUrlString(string urlString)
		{
			return Uri.UnescapeDataString(urlString);
		}
	}
}
