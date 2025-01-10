using System;

namespace Unity.Services.Qos.Models
{
	internal interface IOneOf
	{
		Type Type { get; }

		object Value { get; }
	}
}
