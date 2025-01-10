using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Services.Qos.Http;
using UnityEngine.Scripting;

namespace Unity.Services.Qos.Models
{
	[Preserve]
	[DataContract(Name = "ErrorResponseBody")]
	internal class ErrorResponseBody
	{
		[Preserve]
		[DataMember(Name = "type", IsRequired = true, EmitDefaultValue = true)]
		public string Type { get; }

		[Preserve]
		[DataMember(Name = "title", IsRequired = true, EmitDefaultValue = true)]
		public string Title { get; }

		[Preserve]
		[DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
		public int Status { get; }

		[Preserve]
		[DataMember(Name = "code", IsRequired = true, EmitDefaultValue = true)]
		public int Code { get; }

		[Preserve]
		[DataMember(Name = "detail", IsRequired = true, EmitDefaultValue = true)]
		public string Detail { get; }

		[Preserve]
		[DataMember(Name = "instance", EmitDefaultValue = false)]
		public string Instance { get; }

		[Preserve]
		[DataMember(Name = "details", EmitDefaultValue = false)]
		public List<IDeserializable> Details { get; }

		[Preserve]
		public ErrorResponseBody(string type, string title, int status, int code, string detail, string instance = null, List<object> details = null)
		{
			Type = type;
			Title = title;
			Status = status;
			Code = code;
			Detail = detail;
			Instance = instance;
			Details = JsonObject.GetNewJsonObjectResponse(details);
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			if (Type != null)
			{
				text = text + "type," + Type + ",";
			}
			if (Title != null)
			{
				text = text + "title," + Title + ",";
			}
			text = text + "status," + Status + ",";
			text = text + "code," + Code + ",";
			if (Detail != null)
			{
				text = text + "detail," + Detail + ",";
			}
			if (Instance != null)
			{
				text = text + "instance," + Instance + ",";
			}
			if (Details != null)
			{
				text = text + "details," + Details.ToString();
			}
			return text;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (Type != null)
			{
				string value = Type.ToString();
				dictionary.Add("type", value);
			}
			if (Title != null)
			{
				string value2 = Title.ToString();
				dictionary.Add("title", value2);
			}
			string value3 = Status.ToString();
			dictionary.Add("status", value3);
			string value4 = Code.ToString();
			dictionary.Add("code", value4);
			if (Detail != null)
			{
				string value5 = Detail.ToString();
				dictionary.Add("detail", value5);
			}
			if (Instance != null)
			{
				string value6 = Instance.ToString();
				dictionary.Add("instance", value6);
			}
			if (Details != null)
			{
				string value7 = Details.ToString();
				dictionary.Add("details", value7);
			}
			return dictionary;
		}
	}
}
