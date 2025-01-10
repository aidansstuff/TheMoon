using System;
using System.Runtime.InteropServices;
using Dissonance.Audio.Capture;
using JetBrains.Annotations;

namespace Dissonance.Audio
{
	public static class AecDiagnostics
	{
		public struct AecStats
		{
			public float DelayMedian;

			public float DelayStdDev;

			public float FractionPoorDelays;

			public float EchoReturnLossAverage;

			public float EchoReturnLossMin;

			public float EchoReturnLossMax;

			public float EchoReturnLossEnhancementAverage;

			public float EchoReturnLossEnhancementMin;

			public float EchoReturnLossEnhancementMax;

			public float ResidualEchoLikelihood;
		}

		public enum AecState
		{
			FilterNotRunning = 0,
			FilterNoInstance = 1,
			FilterNoSamplesSubmitted = 2,
			FilterOk = 3
		}

		private const string ImportString = "AudioPluginDissonance";

		private const CallingConvention Convention = CallingConvention.Cdecl;

		[DllImport("AudioPluginDissonance", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Dissonance_GetAecMetrics(IntPtr buffer, int length);

		public static AecState GetState()
		{
			return (AecState)WebRtcPreprocessingPipeline.GetAecFilterState();
		}

		public static AecStats GetStats([CanBeNull] ref float[] temp)
		{
			if (temp == null || temp.Length < 10)
			{
				temp = new float[10];
			}
			GCHandle gCHandle = GCHandle.Alloc(temp, GCHandleType.Pinned);
			try
			{
				Dissonance_GetAecMetrics(gCHandle.AddrOfPinnedObject(), temp.Length);
			}
			finally
			{
				gCHandle.Free();
			}
			AecStats result = default(AecStats);
			result.DelayMedian = temp[0];
			result.DelayStdDev = temp[1];
			result.FractionPoorDelays = temp[2];
			result.EchoReturnLossAverage = temp[3];
			result.EchoReturnLossMin = temp[4];
			result.EchoReturnLossMax = temp[5];
			result.EchoReturnLossEnhancementAverage = temp[6];
			result.EchoReturnLossEnhancementMin = temp[7];
			result.EchoReturnLossEnhancementMax = temp[8];
			result.ResidualEchoLikelihood = temp[9];
			return result;
		}
	}
}
