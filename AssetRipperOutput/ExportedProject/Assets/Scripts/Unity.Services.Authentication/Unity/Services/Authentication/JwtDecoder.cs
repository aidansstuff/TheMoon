using System;
using System.Text;

namespace Unity.Services.Authentication
{
	internal class JwtDecoder : IJwtDecoder
	{
		private static readonly char[] k_JwtSeparator = new char[1] { '.' };

		public T Decode<T>(string token) where T : BaseJwt
		{
			string[] array = token.Split(k_JwtSeparator);
			if (array.Length == 3)
			{
				string input = array[1];
				return IsolatedJsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(Base64UrlDecode(input)), SerializerSettings.DefaultSerializerSettings);
			}
			Logger.LogError($"That is not a valid token (expected 3 parts but has {array.Length}).");
			return null;
		}

		private byte[] Base64UrlDecode(string input)
		{
			string text = input;
			text = text.Replace('-', '+');
			text = text.Replace('_', '/');
			int num = input.Length % 4;
			if (num > 0)
			{
				text += new string('=', 4 - num);
			}
			return Convert.FromBase64String(text);
		}
	}
}
