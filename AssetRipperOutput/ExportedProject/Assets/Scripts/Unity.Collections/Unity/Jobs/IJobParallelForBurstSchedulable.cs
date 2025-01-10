using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Jobs
{
	[JobProducerType(typeof(IJobParallelForExtensionsBurstSchedulable.JobParallelForBurstSchedulableProducer<>))]
	public interface IJobParallelForBurstSchedulable
	{
		void Execute(int index);
	}
}
