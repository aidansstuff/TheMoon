using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("This storage will no longer be used. (RemovedAfter 2021-06-01)")]
	public struct Words
	{
		private int Index;

		public void ToFixedString<T>(ref T value) where T : IUTF8Bytes, INativeList<byte>
		{
			WordStorage.Instance.GetFixedString(Index, ref value);
		}

		public override string ToString()
		{
			FixedString512Bytes value = default(FixedString512Bytes);
			ToFixedString(ref value);
			return value.ToString();
		}

		public void SetFixedString<T>(ref T value) where T : IUTF8Bytes, INativeList<byte>
		{
			Index = WordStorage.Instance.GetOrCreateIndex(ref value);
		}

		public void SetString(string value)
		{
			FixedString512Bytes value2 = value;
			SetFixedString(ref value2);
		}
	}
}
