namespace Steamworks.Data
{
	public struct DepotId
	{
		public uint Value;

		public static implicit operator DepotId(uint value)
		{
			DepotId result = default(DepotId);
			result.Value = value;
			return result;
		}

		public static implicit operator DepotId(int value)
		{
			DepotId result = default(DepotId);
			result.Value = (uint)value;
			return result;
		}

		public static implicit operator uint(DepotId value)
		{
			return value.Value;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
