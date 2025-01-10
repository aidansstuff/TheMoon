namespace Unity.Netcode
{
	public enum NetworkDelivery
	{
		Unreliable = 0,
		UnreliableSequenced = 1,
		Reliable = 2,
		ReliableSequenced = 3,
		ReliableFragmentedSequenced = 4
	}
}
