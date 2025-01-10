using System;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication.Generated
{
	[DataContract(Name = "UpdateNameRequest")]
	[Preserve]
	internal class UpdateNameRequest
	{
		[DataMember(Name = "name", IsRequired = true, EmitDefaultValue = true)]
		[Preserve]
		public string Name { get; set; }

		[Preserve]
		public UpdateNameRequest(string name = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name is a required property for UpdateNameRequest and cannot be null");
			}
			Name = name;
		}
	}
}
