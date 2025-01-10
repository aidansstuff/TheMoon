using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace Unity.Services.Qos.QosDiscovery
{
	[Preserve]
	internal class QosDiscoveryApiBaseRequest
	{
		private static readonly Regex JsonRegex = new Regex("application\\/json(;\\s)?((charset=utf8|q=[0-1]\\.\\d)(\\s)?)*");

		[Preserve]
		public List<string> AddParamsToQueryParams(List<string> queryParams, string key, string value)
		{
			key = UnityWebRequest.EscapeURL(key);
			value = UnityWebRequest.EscapeURL(value);
			queryParams.Add(key + "=" + value);
			return queryParams;
		}

		[Preserve]
		public List<string> AddParamsToQueryParams(List<string> queryParams, string key, List<string> values, string style, bool explode)
		{
			if (explode)
			{
				foreach (string value in values)
				{
					string text = UnityWebRequest.EscapeURL(value);
					queryParams.Add(UnityWebRequest.EscapeURL(key) + "=" + text);
				}
			}
			else
			{
				string text2 = UnityWebRequest.EscapeURL(key) + "=";
				foreach (string value2 in values)
				{
					text2 = text2 + UnityWebRequest.EscapeURL(value2) + ",";
				}
				text2 = text2.Remove(text2.Length - 1);
				queryParams.Add(text2);
			}
			return queryParams;
		}

		[Preserve]
		public List<string> AddParamsToQueryParams(List<string> queryParams, Dictionary<string, string> modelVars)
		{
			foreach (string key in modelVars.Keys)
			{
				string text = UnityWebRequest.EscapeURL(modelVars[key]);
				queryParams.Add(UnityWebRequest.EscapeURL(key) + "=" + text);
			}
			return queryParams;
		}

		[Preserve]
		public List<string> AddParamsToQueryParams<T>(List<string> queryParams, string key, T value)
		{
			if (queryParams == null)
			{
				queryParams = new List<string>();
			}
			key = UnityWebRequest.EscapeURL(key);
			string text = UnityWebRequest.EscapeURL(value.ToString());
			queryParams.Add(key + "=" + text);
			return queryParams;
		}

		[Preserve]
		public string GetPathParamString(List<string> pathParam)
		{
			string text = "";
			foreach (string item in pathParam)
			{
				text = text + UnityWebRequest.EscapeURL(item) + ",";
			}
			return text.Remove(text.Length - 1);
		}

		public byte[] ConstructBody(Stream stream)
		{
			if (stream != null)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					return memoryStream.ToArray();
				}
			}
			return null;
		}

		public byte[] ConstructBody(string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		public byte[] ConstructBody(object o)
		{
			return JsonSerialization.Serialize(o);
		}

		public string GenerateAcceptHeader(string[] accepts)
		{
			if (accepts.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < accepts.Length; i++)
			{
				if (string.Equals(accepts[i], "application/json", StringComparison.OrdinalIgnoreCase))
				{
					return "application/json";
				}
			}
			return string.Join(", ", accepts);
		}

		public string GenerateContentTypeHeader(string[] contentTypes)
		{
			if (contentTypes.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < contentTypes.Length; i++)
			{
				if (!string.IsNullOrWhiteSpace(contentTypes[i]) && JsonRegex.IsMatch(contentTypes[i]))
				{
					return contentTypes[i];
				}
			}
			return contentTypes[0];
		}

		public IMultipartFormSection GenerateMultipartFormFileSection(string paramName, FileStream stream, string contentType)
		{
			return new MultipartFormFileSection(paramName, ConstructBody(stream), GetFileName(stream.Name), contentType);
		}

		public IMultipartFormSection GenerateMultipartFormFileSection(string paramName, Stream stream, string contentType)
		{
			return new MultipartFormFileSection(paramName, ConstructBody(stream), Guid.NewGuid().ToString(), contentType);
		}

		private string GetFileName(string filePath)
		{
			return Path.GetFileName(filePath);
		}
	}
}
