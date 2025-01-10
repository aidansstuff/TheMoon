using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Jobs
{
	[JobProducerType(typeof(JobParallelIndexListExtensions.JobParallelForFilterProducer<>))]
	public interface IJobParallelForFilter
	{
		bool Execute(int index);
	}
}
