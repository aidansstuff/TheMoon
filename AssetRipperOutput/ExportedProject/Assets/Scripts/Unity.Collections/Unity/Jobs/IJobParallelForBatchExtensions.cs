using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace Unity.Jobs
{
	public static class IJobParallelForBatchExtensions
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct JobParallelForBatchProducer<T> where T : struct, IJobParallelForBatch
		{
			internal delegate void ExecuteJobFunction(ref T jobData, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);

			internal static readonly SharedStatic<IntPtr> jobReflectionData = SharedStatic<IntPtr>.GetOrCreate<JobParallelForBatchProducer<T>>();

			[Preserve]
			internal static void Initialize()
			{
				if (jobReflectionData.Data == IntPtr.Zero)
				{
					jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(T), new ExecuteJobFunction(Execute));
				}
			}

			public static void Execute(ref T jobData, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
			{
				int beginIndex;
				int endIndex;
				while (JobsUtility.GetWorkStealingRange(ref ranges, jobIndex, out beginIndex, out endIndex))
				{
					jobData.Execute(beginIndex, endIndex - beginIndex);
				}
			}
		}

		public static void EarlyJobInit<T>() where T : struct, IJobParallelForBatch
		{
			JobParallelForBatchProducer<T>.Initialize();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckReflectionDataCorrect(IntPtr reflectionData)
		{
			if (reflectionData == IntPtr.Zero)
			{
				throw new InvalidOperationException("Reflection data was not set up by an Initialize() call");
			}
		}

		public unsafe static JobHandle ScheduleBatch<T>(this T jobData, int arrayLength, int minIndicesPerJobCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForBatch
		{
			IntPtr data = JobParallelForBatchProducer<T>.jobReflectionData.Data;
			JobsUtility.JobScheduleParameters parameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref jobData), data, dependsOn, ScheduleMode.Batched);
			return JobsUtility.ScheduleParallelFor(ref parameters, arrayLength, minIndicesPerJobCount);
		}

		public unsafe static void RunBatch<T>(this T jobData, int arrayLength) where T : struct, IJobParallelForBatch
		{
			IntPtr data = JobParallelForBatchProducer<T>.jobReflectionData.Data;
			JobsUtility.JobScheduleParameters parameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref jobData), data, default(JobHandle), ScheduleMode.Run);
			JobsUtility.ScheduleParallelFor(ref parameters, arrayLength, arrayLength);
		}
	}
}
