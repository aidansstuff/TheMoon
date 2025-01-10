using System.Text;
using Newtonsoft.Json;

namespace Unity.Services.Qos.QosDiscovery
{
	internal static class JsonSerialization
	{
		public static byte[] Serialize<T>(T obj)
		{
			return Encoding.UTF8.GetBytes(SerializeToString(obj));
		}

		public static string SerializeToString<T>(T obj)
		{
			return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});
		}
	}
}
