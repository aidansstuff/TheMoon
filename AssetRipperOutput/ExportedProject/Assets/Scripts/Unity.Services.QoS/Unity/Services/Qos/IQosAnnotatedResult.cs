using System.Collections.Generic;

namespace Unity.Services.Qos
{
	public interface IQosAnnotatedResult : IQosResult
	{
		Dictionary<string, List<string>> Annotations { get; }
	}
}
