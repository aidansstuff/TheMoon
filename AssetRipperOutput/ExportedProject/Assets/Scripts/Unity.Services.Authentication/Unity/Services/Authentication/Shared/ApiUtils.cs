using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Unity.Services.Authentication.Shared
{
	internal class ApiUtils
	{
		public static readonly Regex JsonRegex = new Regex("(?i)^(application/json|[^;/ \t]+/[^;/ \t]+[+]json)[ \t]*(;.*)?$");

		public static Multimap<string, string> ParameterToMultiMap(IApiConfiguration configuration, string collectionFormat, string name, object value)
		{
			Multimap<string, string> multimap = new Multimap<string, string>();
			if (value is ICollection collection && collectionFormat == "multi")
			{
				foreach (object item in collection)
				{
					multimap.Add(name, ParameterToString(configuration, item));
				}
			}
			else if (value is IDictionary dictionary)
			{
				if (collectionFormat == "deepObject")
				{
					foreach (DictionaryEntry item2 in dictionary)
					{
						multimap.Add(name + "[" + item2.Key?.ToString() + "]", ParameterToString(configuration, item2.Value));
					}
				}
				else
				{
					foreach (DictionaryEntry item3 in dictionary)
					{
						multimap.Add(item3.Key.ToString(), ParameterToString(configuration, item3.Value));
					}
				}
			}
			else
			{
				multimap.Add(name, ParameterToString(configuration, value));
			}
			return multimap;
		}

		public static string ParameterToString(IApiConfiguration configuration, object obj)
		{
			if (obj is DateTime dateTime)
			{
				return dateTime.ToString(configuration.DateTimeFormat);
			}
			if (obj is DateTimeOffset dateTimeOffset)
			{
				return dateTimeOffset.ToString(configuration.DateTimeFormat);
			}
			if (obj is bool)
			{
				if (!(bool)obj)
				{
					return "false";
				}
				return "true";
			}
			if (obj is ICollection source)
			{
				return string.Join(",", source.Cast<object>());
			}
			if (obj is Enum && HasEnumMemberAttrValue(obj))
			{
				return GetEnumMemberAttrValue(obj);
			}
			return Convert.ToString(obj, CultureInfo.InvariantCulture);
		}

		public static string Base64Encode(string text)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
		}

		public static string SelectHeaderContentType(string[] contentTypes)
		{
			if (contentTypes.Length == 0)
			{
				return null;
			}
			foreach (string text in contentTypes)
			{
				if (IsJsonMime(text))
				{
					return text;
				}
			}
			return contentTypes[0];
		}

		public static string SelectHeaderAccept(string[] accepts)
		{
			if (accepts.Length == 0)
			{
				return null;
			}
			if (accepts.Contains("application/json", StringComparer.OrdinalIgnoreCase))
			{
				return "application/json";
			}
			return string.Join(",", accepts);
		}

		public static bool IsJsonMime(string mime)
		{
			if (string.IsNullOrWhiteSpace(mime))
			{
				return false;
			}
			if (!JsonRegex.IsMatch(mime))
			{
				return mime.Equals("application/json-patch+json");
			}
			return true;
		}

		private static bool HasEnumMemberAttrValue(object enumVal)
		{
			if (enumVal == null)
			{
				throw new ArgumentNullException("enumVal");
			}
			if (enumVal.GetType().GetMember(enumVal.ToString() ?? throw new InvalidOperationException()).FirstOrDefault()?.GetCustomAttributes(inherit: false).OfType<EnumMemberAttribute>().FirstOrDefault() != null)
			{
				return true;
			}
			return false;
		}

		private static string GetEnumMemberAttrValue(object enumVal)
		{
			if (enumVal == null)
			{
				throw new ArgumentNullException("enumVal");
			}
			return (enumVal.GetType().GetMember(enumVal.ToString() ?? throw new InvalidOperationException()).FirstOrDefault()?.GetCustomAttributes(inherit: false).OfType<EnumMemberAttribute>().FirstOrDefault())?.Value;
		}
	}
}
