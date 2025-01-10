using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication
{
	internal class BaseJwt
	{
		[JsonProperty("exp")]
		public int ExpirationTimeUnix;

		[JsonProperty("iat")]
		public int IssuedAtTimeUnix;

		[JsonProperty("nbf")]
		public int NotBeforeTimeUnix;

		[JsonIgnore]
		public DateTime? ExpirationTime => ConvertTimestamp(ExpirationTimeUnix);

		[JsonIgnore]
		public DateTime? IssuedAtTime => ConvertTimestamp(IssuedAtTimeUnix);

		[JsonIgnore]
		public DateTime? NotBeforeTime => ConvertTimestamp(NotBeforeTimeUnix);

		[Preserve]
		public BaseJwt()
		{
		}

		protected DateTime? ConvertTimestamp(int timestamp)
		{
			if (timestamp != 0)
			{
				return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
			}
			return null;
		}
	}
}
