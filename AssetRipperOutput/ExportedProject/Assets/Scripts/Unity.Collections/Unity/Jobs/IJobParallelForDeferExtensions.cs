using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace Unity.Jobs
{
	public static class IJobParallelForDeferExtensions
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct JobParallelForDeferProducer<T> where T : struct, IJobParallelForDefer
		{
			public delegate void ExecuteJobFunction(ref T jobData, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);

			internal static readonly SharedStatic<IntPtr> jobReflectionData = SharedStatic<IntPtr>.GetOrCreate<JobParallelForDeferProducer<T>>();

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
					for (int i = beginIndex; i < endIndex; i++)
					{
						jobData.Execute(i);
					}
				}
			}
		}

		public static void EarlyJobInit<T>() where T : struct, IJobParallelForDefer
		{
			JobParallelForDeferProducer<T>.Initialize();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckReflectionDataCorrect(IntPtr reflectionData)
		{
			if (reflectionData == IntPtr.Zero)
			{
				throw new InvalidOperationException("Reflection data was not set up by a call to Initialize()");
			}
		}

		public unsafe static JobHandle Schedule<T, U>(this T jobData, NativeList<U> list, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForDefer where U : unmanaged
		{
			void* atomicSafetyHandlePtr = null;
			return ScheduleInternal(ref jobData, innerloopBatchCount, NativeListUnsafeUtility.GetInternalListDataPtrUnchecked(ref list), atomicSafetyHandlePtr, dependsOn);
		}

		public unsafe static JobHandle ScheduleByRef<T, U>(this ref T jobData, NativeList<U> list, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForDefer where U : unmanaged
		{
			void* atomicSafetyHandlePtr = null;
			return ScheduleInternal(ref jobData, innerloopBatchCount, NativeListUnsafeUtility.GetInternalListDataPtrUnchecked(ref list), atomicSafetyHandlePtr, dependsOn);
		}

		public unsafe static JobHandle Schedule<T>(this T jobData, int* forEachCount, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForDefer
		{
			byte* forEachListPtr = (byte*)forEachCount - sizeof(void*);
			return ScheduleInternal(ref jobData, innerloopBatchCount, forEachListPtr, null, dependsOn);
		}

		public unsafe static JobHandle ScheduleByRef<T>(this ref T jobData, int* forEachCount, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForDefer
		{
			byte* forEachListPtr = (byte*)forEachCount - sizeof(void*);
			return ScheduleInternal(ref jobData, innerloopBatchCount, forEachListPtr, null, dependsOn);
		}

		private unsafe static JobHandle ScheduleInternal<T>(ref T jobData, int innerloopBatchCount, void* forEachListPtr, void* atomicSafetyHandlePtr, JobHandle dependsOn) where T : struct, IJobParallelForDefer
		{
			IntPtr data = JobParallelForDeferProducer<T>.jobReflectionData.Data;
			JobsUtility.JobScheduleParameters parameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref jobData), data, dependsOn, ScheduleMode.Batched);
			return JobsUtility.ScheduleParallelForDeferArraySize(ref parameters, innerloopBatchCount, forEachListPtr, atomicSafetyHandlePtr);
		}
	}
}
