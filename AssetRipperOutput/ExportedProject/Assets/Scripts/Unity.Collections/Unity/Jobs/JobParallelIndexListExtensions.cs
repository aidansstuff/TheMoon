using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Unity.Jobs
{
	public static class JobParallelIndexListExtensions
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct JobParallelForFilterProducer<T> where T : struct, IJobParallelForFilter
		{
			public struct JobWrapper
			{
				[NativeDisableParallelForRestriction]
				public NativeList<int> outputIndices;

				public int appendCount;

				public T JobData;
			}

			public delegate void ExecuteJobFunction(ref JobWrapper jobWrapper, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);

			internal static readonly SharedStatic<IntPtr> jobReflectionData = SharedStatic<IntPtr>.GetOrCreate<JobParallelForFilterProducer<T>>();

			[Preserve]
			public static void Initialize()
			{
				if (jobReflectionData.Data == IntPtr.Zero)
				{
					jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(JobWrapper), typeof(T), new ExecuteJobFunction(Execute));
				}
			}

			public static void Execute(ref JobWrapper jobWrapper, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
			{
				if (jobWrapper.appendCount == -1)
				{
					ExecuteFilter(ref jobWrapper, bufferRangePatchData);
				}
				else
				{
					ExecuteAppend(ref jobWrapper, bufferRangePatchData);
				}
			}

			public unsafe static void ExecuteAppend(ref JobWrapper jobWrapper, IntPtr bufferRangePatchData)
			{
				int length = jobWrapper.outputIndices.Length;
				jobWrapper.outputIndices.Capacity = math.max(jobWrapper.appendCount + length, jobWrapper.outputIndices.Capacity);
				int* unsafePtr = (int*)jobWrapper.outputIndices.GetUnsafePtr();
				int num = length;
				for (int i = 0; i != jobWrapper.appendCount; i++)
				{
					if (jobWrapper.JobData.Execute(i))
					{
						unsafePtr[num] = i;
						num++;
					}
				}
				jobWrapper.outputIndices.ResizeUninitialized(num);
			}

			public unsafe static void ExecuteFilter(ref JobWrapper jobWrapper, IntPtr bufferRangePatchData)
			{
				int* unsafePtr = (int*)jobWrapper.outputIndices.GetUnsafePtr();
				int length = jobWrapper.outputIndices.Length;
				int num = 0;
				for (int i = 0; i != length; i++)
				{
					int num2 = unsafePtr[i];
					if (jobWrapper.JobData.Execute(num2))
					{
						unsafePtr[num] = num2;
						num++;
					}
				}
				jobWrapper.outputIndices.ResizeUninitialized(num);
			}
		}

		public static void EarlyJobInit<T>() where T : struct, IJobParallelForFilter
		{
			JobParallelForFilterProducer<T>.Initialize();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckReflectionDataCorrect(IntPtr reflectionData)
		{
			if (reflectionData == IntPtr.Zero)
			{
				throw new InvalidOperationException("Reflection data was not set up by a call to Initialize()");
			}
		}

		public unsafe static JobHandle ScheduleAppend<T>(this T jobData, NativeList<int> indices, int arrayLength, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForFilter
		{
			JobParallelForFilterProducer<T>.JobWrapper jobWrapper = default(JobParallelForFilterProducer<T>.JobWrapper);
			jobWrapper.JobData = jobData;
			jobWrapper.outputIndices = indices;
			jobWrapper.appendCount = arrayLength;
			JobParallelForFilterProducer<T>.JobWrapper output = jobWrapper;
			IntPtr data = JobParallelForFilterProducer<T>.jobReflectionData.Data;
			JobsUtility.JobScheduleParameters parameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref output), data, dependsOn, ScheduleMode.Single);
			return JobsUtility.Schedule(ref parameters);
		}

		public unsafe static JobHandle ScheduleFilter<T>(this T jobData, NativeList<int> indices, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForFilter
		{
			JobParallelForFilterProducer<T>.JobWrapper jobWrapper = default(JobParallelForFilterProducer<T>.JobWrapper);
			jobWrapper.JobData = jobData;
			jobWrapper.outputIndices = indices;
			jobWrapper.appendCount = -1;
			JobParallelForFilterProducer<T>.JobWrapper output = jobWrapper;
			IntPtr data = JobParallelForFilterProducer<T>.jobReflectionData.Data;
			JobsUtility.JobScheduleParameters parameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref output), data, dependsOn, ScheduleMode.Single);
			return JobsUtility.Schedule(ref parameters);
		}

		public unsafe static void RunAppend<T>(this T jobData, NativeList<int> indices, int arrayLength) where T : struct, IJobParallelForFilter
		{
			JobParallelForFilterProducer<T>.JobWrapper jobWrapper = default(JobParallelForFilterProducer<T>.JobWrapper);
			jobWrapper.JobData = jobData;
			jobWrapper.outputIndices = indices;
			jobWrapper.appendCount = arrayLength;
			JobParallelForFilterProducer<T>.JobWrapper output = jobWrapper;
			IntPtr data = JobParallelForFilterProducer<T>.jobReflectionData.Data;
			JobsUtility.JobScheduleParameters parameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref output), data, default(JobHandle), ScheduleMode.Run);
			JobsUtility.Schedule(ref parameters);
		}

		public unsafe static void RunFilter<T>(this T jobData, NativeList<int> indices) where T : struct, IJobParallelForFilter
		{
			JobParallelForFilterProducer<T>.JobWrapper jobWrapper = default(JobParallelForFilterProducer<T>.JobWrapper);
			jobWrapper.JobData = jobData;
			jobWrapper.outputIndices = indices;
			jobWrapper.appendCount = -1;
			JobParallelForFilterProducer<T>.JobWrapper output = jobWrapper;
			IntPtr data = JobParallelForFilterProducer<T>.jobReflectionData.Data;
			JobsUtility.JobScheduleParameters parameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref output), data, default(JobHandle), ScheduleMode.Run);
			JobsUtility.Schedule(ref parameters);
		}
	}
}
