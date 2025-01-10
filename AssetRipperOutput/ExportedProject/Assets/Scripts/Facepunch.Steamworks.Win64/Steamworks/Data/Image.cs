using System;

namespace Steamworks.Data
{
	public struct Image
	{
		public uint Width;

		public uint Height;

		public byte[] Data;

		public Color GetPixel(int x, int y)
		{
			if (x < 0 || x >= Width)
			{
				throw new Exception("x out of bounds");
			}
			if (y < 0 || y >= Height)
			{
				throw new Exception("y out of bounds");
			}
			Color result = default(Color);
			long num = (y * Width + x) * 4;
			result.r = Data[num];
			result.g = Data[num + 1];
			result.b = Data[num + 2];
			result.a = Data[num + 3];
			return result;
		}

		public override string ToString()
		{
			return $"{Width}x{Height} ({Data.Length}bytes)";
		}
	}
}
