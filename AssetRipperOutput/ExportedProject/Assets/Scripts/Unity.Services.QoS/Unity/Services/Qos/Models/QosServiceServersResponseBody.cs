using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Qos.Models
{
	[Preserve]
	[DataContract(Name = "QosServiceServersResponseBody")]
	internal class QosServiceServersResponseBody
	{
		[Preserve]
		[DataMember(Name = "data", IsRequired = true, EmitDefaultValue = true)]
		public QosServiceServersList Data { get; }

		[Preserve]
		public QosServiceServersResponseBody(QosServiceServersList data)
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
