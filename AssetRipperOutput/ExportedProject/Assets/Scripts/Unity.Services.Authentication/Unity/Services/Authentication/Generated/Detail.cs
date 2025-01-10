using System;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication.Generated
{
	[DataContract(Name = "Detail")]
	[Preserve]
	internal class Detail
	{
		[DataMember(Name = "errorType", IsRequired = true, EmitDefaultValue = true)]
		[Preserve]
		public string ErrorType { get; set; }

		[DataMember(Name = "message", IsRequired = true, EmitDefaultValue = true)]
		[Preserve]
		public string Message { get; set; }

		[Preserve]
		public Detail(string errorType = null, string message = null)
		{
			if (errorType == null)
			{
				throw new ArgumentNullException("errorType is a required property for Detail and cannot be null");
			}
			ErrorType = errorType;
			if (message == null)
			{
				throw new ArgumentNullException("message is a required property for Detail and cannot be null");
			}
			Message = message;
		}
	}
}
