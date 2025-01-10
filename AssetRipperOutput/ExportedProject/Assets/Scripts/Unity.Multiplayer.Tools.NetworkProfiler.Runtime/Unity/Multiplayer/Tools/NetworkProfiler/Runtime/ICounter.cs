namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
	internal interface ICounter
	{
		void Sample(long inValue);
	}
}
