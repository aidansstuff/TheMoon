namespace Unity.Multiplayer.Tools.NetStats
{
	internal interface IResettable
	{
		bool ShouldResetOnDispatch { get; }

		void Reset();
	}
}
