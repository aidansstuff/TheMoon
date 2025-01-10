using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Relay.Models
{
	[Preserve]
	[DataContract(Name = "ErrorResponseBody")]
	public class ErrorResponseBody
	{
		[Preserve]
		[DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
		public int Status { get; }

		[Preserve]
		[DataMember(Name = "detail", IsRequired = true, EmitDefaultValue = true)]
		public string Detail { get; }

		[Preserve]
		[DataMember(Name = "title", IsRequired = true, EmitDefaultValue = true)]
		public string Title { get; }

		[Preserve]
		[DataMember(Name = "details", EmitDefaultValue = false)]
		public List<KeyValuePair> Details { get; }

		[Preserve]
		[DataMember(Name = "type", IsRequired = true, EmitDefaultValue = true)]
		public string Type { get; }

		[Preserve]
		[DataMember(Name = "code", IsRequired = true, EmitDefaultValue = true)]
		public int Code { get; }

		[Preserve]
		public ErrorResponseBody(int status, string detail, string title, string type, int code, List<KeyValuePair> details = null)
		{
			Status = status;
			Detail = detail;
			Title = title;
			Details = details;
			Type = type;
			Code = code;
		}

		internal string SerializeAsPathParam()
		{
			string text = "";
			text = text + "status," + Status + ",";
			if (Detail != null)
			{
				text = text + "detail," + Detail + ",";
			}
			if (Title != null)
			{
				text = text + "title," + Title + ",";
			}
			if (Details != null)
			{
				text = text + "details," + Details.ToString() + ",";
			}
			if (Type != null)
			{
				text = text + "type," + Type + ",";
			}
			return text + "code," + Code;
		}

		internal Dictionary<string, string> GetAsQueryParam()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string value = Status.ToString();
			dictionary.Add("status", value);
			if (Detail != null)
			{
				string value2 = Detail.ToString();
				dictionary.Add("detail", value2);
			}
			if (Title != null)
			{
				string value3 = Title.ToString();
				dictionary.Add("title", value3);
			}
			if (Type != null)
			{
				string value4 = Type.ToString();
				dictionary.Add("type", value4);
			}
			string value5 = Code.ToString();
			dictionary.Add("code", value5);
			return dictionary;
		}
	}
}
