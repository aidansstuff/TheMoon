using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Jobs
{
	[JobProducerType(typeof(IJobBurstSchedulableExtensions.JobBurstSchedulableProducer<>))]
	public interface IJobBurstSchedulable
	{
		void Execute();
	}
}
