using Unity.Collections;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	internal static class StringConversionUtility
	{
		public unsafe static FixedString64Bytes ConvertToFixedString(string value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			if (FixedString64Bytes.UTF8MaxLengthInBytes < value.Length)
			{
				FixedString64Bytes result = default(FixedString64Bytes);
				fixed (char* src = value)
				{
					UTF8ArrayUnsafeUtility.Copy(result.GetUnsafePtr(), out var destLength, FixedString64Bytes.UTF8MaxLengthInBytes, src, value.Length);
					result.Length = destLength;
				}
				return result;
			}
			return value;
		}
	}
}
