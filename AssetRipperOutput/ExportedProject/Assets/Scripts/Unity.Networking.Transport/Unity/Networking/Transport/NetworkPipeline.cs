namespace Unity.Networking.Transport
{
	public struct NetworkPipeline
	{
		internal int Id;

		public static NetworkPipeline Null => default(NetworkPipeline);

		public static bool operator ==(NetworkPipeline lhs, NetworkPipeline rhs)
		{
			return lhs.Id == rhs.Id;
		}

		public static bool operator !=(NetworkPipeline lhs, NetworkPipeline rhs)
		{
			return lhs.Id != rhs.Id;
		}

		public override bool Equals(object compare)
		{
			return this == (NetworkPipeline)compare;
		}

		public override int GetHashCode()
		{
			return Id;
		}

		public bool Equals(NetworkPipeline connection)
		{
			return connection.Id == Id;
		}
	}
}
