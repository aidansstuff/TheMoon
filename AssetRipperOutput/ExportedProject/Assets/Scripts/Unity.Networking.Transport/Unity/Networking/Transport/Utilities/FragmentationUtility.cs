using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.Networking.Transport.Utilities
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct FragmentationUtility
	{
		public struct Parameters : INetworkParameter
		{
			internal const int k_DefaultPayloadCapacity = 4096;

			public int PayloadCapacity;

			public bool Validate()
			{
				bool result = true;
				if (PayloadCapacity <= 0)
				{
					result = false;
					Debug.LogError(string.Format("{0} value ({1}) must be greater than 0", "PayloadCapacity", PayloadCapacity));
				}
				return result;
			}
		}
	}
}
