using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.QoS;

namespace Unity.Services.Qos.Runner
{
	internal interface IQosJob : IJob
	{
		NativeArray<InternalQosResult> QosResults { get; }

		JobHandle Schedule<T>(JobHandle dependsOn = default(JobHandle)) where T : struct, IJob;

		void Dispose();
	}
}
