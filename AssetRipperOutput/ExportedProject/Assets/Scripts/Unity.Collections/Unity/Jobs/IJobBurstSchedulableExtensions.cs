using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace Unity.Jobs
{
	public static class IJobBurstSchedulableExtensions
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct JobBurstSchedulableProducer<T> where T : struct, IJobBurstSchedulable
		{
			internal delegate void ExecuteJobFunction(ref T data, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);

			internal static readonly SharedStatic<IntPtr> jobReflectionData = SharedStatic<IntPtr>.GetOrCreate<JobBurstSchedulableProducer<T>>();

			[Preserve]
			internal static void Initialize()
			{
				if (jobReflectionData.Data == IntPtr.Zero)
				{
					jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(T), new ExecuteJobFunction(Execute));
				}
			}

			public static void Execute(ref T data, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
			{
				data.Execute();
			}
		}

		public static void EarlyJobInit<T>() where T : struct, IJobBurstSchedulable
		{
			JobBurstSchedulableProducer<T>.Initialize();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckReflectionDataCorrect(IntPtr reflectionData)
		{
			if (reflectionData == IntPtr.Zero)
			{
				throw new InvalidOperationException("Reflection data was not set up by an Initialize() call");
			}
		}

		public unsafe static JobHandle Schedule<T>(this T jobData, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobBurstSchedulable
		{
			IntPtr data = JobBurstSchedulableProducer<T>.jobReflectionData.Data;
			JobsUtility.JobScheduleParameters parameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref jobData), data, dependsOn, ScheduleMode.Single);
			return JobsUtility.Schedule(ref parameters);
		}

		public unsafe static void Run<T>(this T jobData) where T : struct, IJobBurstSchedulable
		{
			IntPtr data = JobBurstSchedulableProducer<T>.jobReflectionData.Data;
			JobsUtility.JobScheduleParameters parameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref jobData), data, default(JobHandle), ScheduleMode.Run);
			JobsUtility.Schedule(ref parameters);
		}
	}
}
