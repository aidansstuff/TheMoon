using Newtonsoft.Json.Converters;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication.Shared
{
	internal class OpenAPIDateConverter : IsoDateTimeConverter
	{
		[Preserve]
		public OpenAPIDateConverter()
		{
			base.DateTimeFormat = "yyyy-MM-dd";
		}
	}
}
