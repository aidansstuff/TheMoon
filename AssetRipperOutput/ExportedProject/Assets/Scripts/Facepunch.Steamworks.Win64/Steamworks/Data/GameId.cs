namespace Steamworks.Data
{
	public struct GameId
	{
		public ulong Value;

		public static implicit operator GameId(ulong value)
		{
			GameId result = default(GameId);
			result.Value = value;
			return result;
		}

		public static implicit operator ulong(GameId value)
		{
			return value.Value;
		}
	}
}
