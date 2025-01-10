using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication.Generated
{
	[DataContract(Name = "ErrorStatus")]
	[Preserve]
	internal class ErrorStatus
	{
		[DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
		[Preserve]
		public int Status { get; set; }

		[DataMember(Name = "title", IsRequired = true, EmitDefaultValue = true)]
		[Preserve]
		public string Title { get; set; }

		[DataMember(Name = "detail", IsRequired = true, EmitDefaultValue = true)]
		[Preserve]
		public string Detail { get; set; }

		[DataMember(Name = "code", IsRequired = true, EmitDefaultValue = true)]
		[Preserve]
		public int Code { get; set; }

		[DataMember(Name = "details", EmitDefaultValue = false)]
		[Preserve]
		public List<Detail> Details { get; set; }

		[Preserve]
		public ErrorStatus(int status = 0, string title = null, string detail = null, int code = 0, List<Detail> details = null)
		{
			Status = status;
			if (title == null)
			{
				throw new ArgumentNullException("title is a required property for ErrorStatus and cannot be null");
			}
			Title = title;
			if (detail == null)
			{
				throw new ArgumentNullException("detail is a required property for ErrorStatus and cannot be null");
			}
			Detail = detail;
			Code = code;
			Details = details;
		}
	}
}
