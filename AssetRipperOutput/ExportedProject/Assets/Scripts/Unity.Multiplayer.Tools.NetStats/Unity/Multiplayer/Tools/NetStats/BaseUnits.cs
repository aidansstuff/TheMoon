using System;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal readonly struct BaseUnits
	{
		private static readonly char[] k_Superscripts = new char[10] { '⁰', '¹', '²', '³', '⁴', '⁵', '⁶', '⁷', '⁸', '⁹' };

		internal sbyte BytesExponent { get; }

		internal sbyte SecondsExponent { get; }

		internal (string, string) NumeratorAndDenominatorDisplayStrings
		{
			get
			{
				string str2 = "";
				string str3 = "";
				for (BaseUnit baseUnit = BaseUnit.Byte; baseUnit < (BaseUnit)2; baseUnit++)
				{
					sbyte exponent2 = GetExponent(baseUnit);
					if (exponent2 > 0)
					{
						AddUnit(baseUnit, exponent2, ref str2);
					}
					else if (exponent2 < 0)
					{
						AddUnit(baseUnit, Math.Abs(exponent2), ref str3);
					}
				}
				return (str2, str3);
				static void AddUnit(BaseUnit unit, sbyte exponent, ref string str)
				{
					str += unit.GetSymbol();
					if (exponent > 1)
					{
						if (exponent >= 100)
						{
							str += k_Superscripts[exponent / 100];
							exponent %= 100;
						}
						if (exponent >= 10)
						{
							str += k_Superscripts[exponent / 10];
							exponent %= 10;
						}
						str += k_Superscripts[exponent / 10];
					}
				}
			}
		}

		internal string DisplayString
		{
			get
			{
				var (text, text2) = NumeratorAndDenominatorDisplayStrings;
				return text + ((text2 == "") ? "" : ("/" + text2));
			}
		}

		public BaseUnits(sbyte bytesExponent = 0, sbyte secondsExponent = 0)
		{
			BytesExponent = bytesExponent;
			SecondsExponent = secondsExponent;
		}

		public BaseUnits WithSeconds(sbyte seconds)
		{
			return new BaseUnits(BytesExponent, seconds);
		}

		public bool Equals(BaseUnits other)
		{
			if (BytesExponent == other.BytesExponent)
			{
				return SecondsExponent == other.SecondsExponent;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is BaseUnits other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((byte)BytesExponent << 8) | (byte)SecondsExponent;
		}

		internal sbyte GetExponent(BaseUnit unit)
		{
			return unit switch
			{
				BaseUnit.Byte => BytesExponent, 
				BaseUnit.Second => SecondsExponent, 
				_ => throw new ArgumentException($"Unhandled BaseUnit {unit}"), 
			};
		}

		public override string ToString()
		{
			return DisplayString;
		}
	}
}
