using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

[DOTSCompilerGenerated]
internal class __JobReflectionRegistrationOutput__1374711130
{
	public static void CreateJobReflectionData()
	{
		try
		{
			IJobExtensions.EarlyJobInit<NativeListDisposeJob>();
		}
		catch (Exception ex)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex, typeof(NativeListDisposeJob));
		}
		try
		{
			IJobExtensions.EarlyJobInit<NativeQueueDisposeJob>();
		}
		catch (Exception ex2)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex2, typeof(NativeQueueDisposeJob));
		}
		try
		{
			IJobExtensions.EarlyJobInit<NativeReferenceDisposeJob>();
		}
		catch (Exception ex3)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex3, typeof(NativeReferenceDisposeJob));
		}
		try
		{
			IJobExtensions.EarlyJobInit<NativeStream.ConstructJobList>();
		}
		catch (Exception ex4)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex4, typeof(NativeStream.ConstructJobList));
		}
		try
		{
			IJobExtensions.EarlyJobInit<NativeStream.ConstructJob>();
		}
		catch (Exception ex5)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex5, typeof(NativeStream.ConstructJob));
		}
		try
		{
			IJobExtensions.EarlyJobInit<UnsafeDisposeJob>();
		}
		catch (Exception ex6)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex6, typeof(UnsafeDisposeJob));
		}
		try
		{
			IJobExtensions.EarlyJobInit<UnsafeParallelHashMapDataDisposeJob>();
		}
		catch (Exception ex7)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex7, typeof(UnsafeParallelHashMapDataDisposeJob));
		}
		try
		{
			IJobExtensions.EarlyJobInit<UnsafeParallelHashMapDisposeJob>();
		}
		catch (Exception ex8)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex8, typeof(UnsafeParallelHashMapDisposeJob));
		}
		try
		{
			IJobExtensions.EarlyJobInit<UnsafeStream.DisposeJob>();
		}
		catch (Exception ex9)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex9, typeof(UnsafeStream.DisposeJob));
		}
		try
		{
			IJobExtensions.EarlyJobInit<UnsafeStream.ConstructJobList>();
		}
		catch (Exception ex10)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex10, typeof(UnsafeStream.ConstructJobList));
		}
		try
		{
			IJobExtensions.EarlyJobInit<UnsafeStream.ConstructJob>();
		}
		catch (Exception ex11)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex11, typeof(UnsafeStream.ConstructJob));
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void EarlyInit()
	{
		CreateJobReflectionData();
	}
}
