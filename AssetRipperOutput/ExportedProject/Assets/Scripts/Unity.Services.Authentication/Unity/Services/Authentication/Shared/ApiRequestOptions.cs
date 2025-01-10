using System.Collections.Generic;
using System.IO;

namespace Unity.Services.Authentication.Shared
{
	internal class ApiRequestOptions
	{
		public Dictionary<string, string> PathParameters { get; set; }

		public Multimap<string, string> QueryParameters { get; set; }

		public Multimap<string, string> HeaderParameters { get; set; }

		public Dictionary<string, string> FormParameters { get; set; }

		public Multimap<string, Stream> FileParameters { get; set; }

		public string Operation { get; set; }

		public object Data { get; set; }

		public ApiRequestOptions()
		{
			PathParameters = new Dictionary<string, string>();
			QueryParameters = new Multimap<string, string>();
			HeaderParameters = new Multimap<string, string>();
			FormParameters = new Dictionary<string, string>();
			FileParameters = new Multimap<string, Stream>();
		}
	}
}
