using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
	public static class Utility
	{
		private static readonly byte[] readBuffer = new byte[8192];

		internal static T ToType<T>(this IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
			{
				return default(T);
			}
			return (T)Marshal.PtrToStructure(ptr, typeof(T));
		}

		internal static object ToType(this IntPtr ptr, Type t)
		{
			if (ptr == IntPtr.Zero)
			{
				return null;
			}
			return Marshal.PtrToStructure(ptr, t);
		}

		internal static uint Swap(uint x)
		{
			return ((x & 0xFF) << 24) + ((x & 0xFF00) << 8) + ((x & 0xFF0000) >> 8) + ((x & 0xFF000000u) >> 24);
		}

		public static uint IpToInt32(this IPAddress ipAddress)
		{
			return Swap((uint)ipAddress.Address);
		}

		public static IPAddress Int32ToIp(uint ipAddress)
		{
			return new IPAddress(Swap(ipAddress));
		}

		public static string FormatPrice(string currency, double price)
		{
			string text = price.ToString("0.00");
			return currency switch
			{
				"AED" => text + "د.إ", 
				"ARS" => "$" + text + " ARS", 
				"AUD" => "A$" + text, 
				"BRL" => "R$" + text, 
				"CAD" => "C$" + text, 
				"CHF" => "Fr. " + text, 
				"CLP" => "$" + text + " CLP", 
				"CNY" => text + "元", 
				"COP" => "COL$ " + text, 
				"CRC" => "₡" + text, 
				"EUR" => "€" + text, 
				"SEK" => text + "kr", 
				"GBP" => "£" + text, 
				"HKD" => "HK$" + text, 
				"ILS" => "₪" + text, 
				"IDR" => "Rp" + text, 
				"INR" => "₹" + text, 
				"JPY" => "¥" + text, 
				"KRW" => "₩" + text, 
				"KWD" => "KD " + text, 
				"KZT" => text + "₸", 
				"MXN" => "Mex$" + text, 
				"MYR" => "RM " + text, 
				"NOK" => text + " kr", 
				"NZD" => "$" + text + " NZD", 
				"PEN" => "S/. " + text, 
				"PHP" => "₱" + text, 
				"PLN" => text + "zł", 
				"QAR" => "QR " + text, 
				"RUB" => text + "₽", 
				"SAR" => "SR " + text, 
				"SGD" => "S$" + text, 
				"THB" => "฿" + text, 
				"TRY" => "₺" + text, 
				"TWD" => "NT$ " + text, 
				"UAH" => "₴" + text, 
				"USD" => "$" + text, 
				"UYU" => "$U " + text, 
				"VND" => "₫" + text, 
				"ZAR" => "R " + text, 
				_ => text + " " + currency, 
			};
		}

		public static string ReadNullTerminatedUTF8String(this BinaryReader br)
		{
			lock (readBuffer)
			{
				int num = 0;
				byte b;
				while ((b = br.ReadByte()) != 0 && num < readBuffer.Length)
				{
					readBuffer[num] = b;
					num++;
				}
				return Encoding.UTF8.GetString(readBuffer, 0, num);
			}
		}
	}
}
