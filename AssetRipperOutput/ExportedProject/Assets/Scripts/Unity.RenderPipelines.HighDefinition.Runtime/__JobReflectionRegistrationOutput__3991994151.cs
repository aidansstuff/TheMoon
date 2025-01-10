using System;
using Unity.Jobs;
using UnityEditor.Rendering.HighDefinition;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Rendering.HighDefinition;

[Unity.Jobs.DOTSCompilerGenerated]
internal class __JobReflectionRegistrationOutput__3991994151
{
	public static void CreateJobReflectionData()
	{
		try
		{
			IJobParallelForExtensions.EarlyJobInit<WaterAmplitudeEvaluator.ReductionStep>();
		}
		catch (Exception ex)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex, typeof(WaterAmplitudeEvaluator.ReductionStep));
		}
		try
		{
			IJobParallelForExtensions.EarlyJobInit<HDGpuLightsBuilder.CreateGpuLightDataJob>();
		}
		catch (Exception ex2)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex2, typeof(HDGpuLightsBuilder.CreateGpuLightDataJob));
		}
		try
		{
			IJobParallelForExtensions.EarlyJobInit<HDProcessedVisibleLightsBuilder.ProcessVisibleLightJob>();
		}
		catch (Exception ex3)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex3, typeof(HDProcessedVisibleLightsBuilder.ProcessVisibleLightJob));
		}
		try
		{
			IJobParallelForTransformExtensions.EarlyJobInit<DecalSystem.UpdateJob>();
		}
		catch (Exception ex4)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex4, typeof(DecalSystem.UpdateJob));
		}
		try
		{
			IJobParallelForExtensions.EarlyJobInit<WaterCPUSimulation.PhillipsSpectrumInitialization>();
		}
		catch (Exception ex5)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex5, typeof(WaterCPUSimulation.PhillipsSpectrumInitialization));
		}
		try
		{
			IJobParallelForExtensions.EarlyJobInit<WaterCPUSimulation.EvaluateDispersion>();
		}
		catch (Exception ex6)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex6, typeof(WaterCPUSimulation.EvaluateDispersion));
		}
		try
		{
			IJobParallelForExtensions.EarlyJobInit<WaterCPUSimulation.InverseFFT>();
		}
		catch (Exception ex7)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex7, typeof(WaterCPUSimulation.InverseFFT));
		}
		try
		{
			IJobParallelForExtensions.EarlyJobInit<WaterSimulationSearchJob>();
		}
		catch (Exception ex8)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex8, typeof(WaterSimulationSearchJob));
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void EarlyInit()
	{
		CreateJobReflectionData();
	}
}
