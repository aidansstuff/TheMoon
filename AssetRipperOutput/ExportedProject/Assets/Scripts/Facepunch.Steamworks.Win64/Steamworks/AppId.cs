namespace Steamworks
{
	public struct AppId
	{
		public uint Value;

		public override string ToString()
		{
			return Value.ToString();
		}

		public static implicit operator AppId(uint value)
		{
			AppId result = default(AppId);
			result.Value = value;
			return result;
		}

		public static implicit operator AppId(int value)
		{
			AppId result = default(AppId);
			result.Value = (uint)value;
			return result;
		}

		public static implicit operator uint(AppId value)
		{
			return value.Value;
		}
	}
}
