using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class TypeInfo
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct EnumInfoJITCache<TEnum> where TEnum : struct, IConvertible
		{
			public static readonly TEnum[] values;

			public static readonly string[] names;

			public static readonly int length;

			static EnumInfoJITCache()
			{
				if (!typeof(TEnum).IsEnum)
				{
					throw new InvalidOperationException($"{typeof(TEnum)} must be an enum type.");
				}
				names = Enum.GetNames(typeof(TEnum));
				length = names.Length;
				values = new TEnum[length];
				Array array = Enum.GetValues(typeof(TEnum));
				for (int i = 0; i < values.Length; i++)
				{
					values[i] = (TEnum)array.GetValue(i);
				}
			}
		}

		public static TEnum[] GetEnumValues<TEnum>() where TEnum : struct, IConvertible
		{
			return EnumInfoJITCache<TEnum>.values;
		}

		public static int GetEnumLength<TEnum>() where TEnum : struct, IConvertible
		{
			return EnumInfoJITCache<TEnum>.length;
		}

		public static string[] GetEnumNames<TEnum>() where TEnum : struct, IConvertible
		{
			return EnumInfoJITCache<TEnum>.names;
		}

		public static TEnum GetEnumLastValue<TEnum>() where TEnum : struct, IConvertible
		{
			return GetEnumValues<TEnum>()[GetEnumLength<TEnum>() - 1];
		}
	}
}
