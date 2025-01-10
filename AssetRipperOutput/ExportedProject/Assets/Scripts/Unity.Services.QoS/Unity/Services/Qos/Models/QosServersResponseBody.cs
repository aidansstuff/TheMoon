using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Qos.Models
{
	[Preserve]
	[DataContract(Name = "QosServersResponseBody")]
	internal class QosServersResponseBody
	{
		[Preserve]
		[DataMember(Name = "data", IsRequired = true, EmitDefaultValue = true)]
		public QosServersList Data { get; }

		[Preserve]
		public QosServersResponseBody(QosServersList data)
		{
			Data = data;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (Data != null)
			{
				text = text + "data," + Data.ToString();
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			return new Dictionary<string, string>();
		}
	}
}
