namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
	internal interface ICounterFactory
	{
		ICounter Construct(string name);
	}
}
