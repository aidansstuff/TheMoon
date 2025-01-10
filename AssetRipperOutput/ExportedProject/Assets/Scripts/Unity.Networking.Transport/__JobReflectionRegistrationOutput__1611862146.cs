using System;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;

[Unity.Jobs.DOTSCompilerGenerated]
internal class __JobReflectionRegistrationOutput__1611862146
{
	public static void CreateJobReflectionData()
	{
		try
		{
			IJobExtensions.EarlyJobInit<BaselibNetworkInterface.FlushSendJob>();
		}
		catch (Exception ex)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex, typeof(BaselibNetworkInterface.FlushSendJob));
		}
		try
		{
			IJobExtensions.EarlyJobInit<BaselibNetworkInterface.ReceiveJob>();
		}
		catch (Exception ex2)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex2, typeof(BaselibNetworkInterface.ReceiveJob));
		}
		try
		{
			IJobExtensions.EarlyJobInit<IPCNetworkInterface.SendUpdate>();
		}
		catch (Exception ex3)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex3, typeof(IPCNetworkInterface.SendUpdate));
		}
		try
		{
			IJobExtensions.EarlyJobInit<IPCNetworkInterface.ReceiveJob>();
		}
		catch (Exception ex4)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex4, typeof(IPCNetworkInterface.ReceiveJob));
		}
		try
		{
			IJobExtensions.EarlyJobInit<NetworkDriver.UpdateJob>();
		}
		catch (Exception ex5)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex5, typeof(NetworkDriver.UpdateJob));
		}
		try
		{
			IJobExtensions.EarlyJobInit<NetworkDriver.ClearEventQueue>();
		}
		catch (Exception ex6)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex6, typeof(NetworkDriver.ClearEventQueue));
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void EarlyInit()
	{
		CreateJobReflectionData();
	}
}
