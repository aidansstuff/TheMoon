using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AnalogState
	{
		public InputSourceMode EMode;

		public float X;

		public float Y;

		internal byte BActive;

		public bool Active => BActive != 0;
	}
}
