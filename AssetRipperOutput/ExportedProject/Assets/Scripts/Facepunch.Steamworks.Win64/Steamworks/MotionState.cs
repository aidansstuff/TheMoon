using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct MotionState
	{
		public float RotQuatX;

		public float RotQuatY;

		public float RotQuatZ;

		public float RotQuatW;

		public float PosAccelX;

		public float PosAccelY;

		public float PosAccelZ;

		public float RotVelX;

		public float RotVelY;

		public float RotVelZ;
	}
}
