using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace Unity.Jobs
{
	public static class IJobParallelForExtensionsBurstSchedulable
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct JobParallelForBurstSchedulableProducer<T> where T : struct, IJobParallelForBurstSchedulable
		{
			internal delegate void ExecuteJobFunction(ref T data, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);

			internal static readonly SharedStatic<IntPtr> jobReflectionData = SharedStatic<IntPtr>.GetOrCreate<JobParallelForBurstSchedulableProducer<T>>();

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
					int num = endIndex;
					for (int i = beginIndex; i < num; i++)
					{
						jobData.Execute(i);
					}
				}
			}
		}

		public static void EarlyJobInit<T>() where T : struct, IJobParallelForBurstSchedulable
		{
			JobParallelForBurstSchedulableProducer<T>.Initialize();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckReflectionDataCorrect(IntPtr reflectionData)
		{
			if (reflectionData == IntPtr.Zero)
			{
				throw new InvalidOperationException("Reflection data was not set up by a call to Initialize()");
			}
		}

		public unsafe static JobHandle Schedule<T>(this T jobData, int arrayLength, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForBurstSchedulable
		{
			IntPtr data = JobParallelForBurstSchedulableProducer<T>.jobReflectionData.Data;
			JobsUtility.JobScheduleParameters parameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref jobData), data, dependsOn, ScheduleMode.Batched);
			return JobsUtility.ScheduleParallelFor(ref parameters, arrayLength, innerloopBatchCount);
		}

		public unsafe static void Run<T>(this T jobData, int arrayLength) where T : struct, IJobParallelForBurstSchedulable
		{
			IntPtr data = JobParallelForBurstSchedulableProducer<T>.jobReflectionData.Data;
			JobsUtility.JobScheduleParameters parameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref jobData), data, default(JobHandle), ScheduleMode.Run);
			JobsUtility.ScheduleParallelFor(ref parameters, arrayLength, arrayLength);
		}
	}
}
