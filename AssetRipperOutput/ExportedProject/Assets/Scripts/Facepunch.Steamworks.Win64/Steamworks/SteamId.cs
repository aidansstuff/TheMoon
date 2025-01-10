namespace Steamworks
{
	public struct SteamId
	{
		public ulong Value;

		public uint AccountId => (uint)(Value & 0xFFFFFFFFu);

		public bool IsValid => Value != 0;

		public static implicit operator SteamId(ulong value)
		{
			SteamId result = default(SteamId);
			result.Value = value;
			return result;
		}

		public static implicit operator ulong(SteamId value)
		{
			return value.Value;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
