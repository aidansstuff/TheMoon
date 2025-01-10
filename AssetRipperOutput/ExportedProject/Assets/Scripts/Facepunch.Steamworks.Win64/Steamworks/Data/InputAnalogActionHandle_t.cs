using System;

namespace Steamworks.Data
{
	internal struct InputAnalogActionHandle_t : IEquatable<InputAnalogActionHandle_t>, IComparable<InputAnalogActionHandle_t>
	{
		public ulong Value;

		public static implicit operator InputAnalogActionHandle_t(ulong value)
		{
			InputAnalogActionHandle_t result = default(InputAnalogActionHandle_t);
			result.Value = value;
			return result;
		}

		public static implicit operator ulong(InputAnalogActionHandle_t value)
		{
			return value.Value;
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override bool Equals(object p)
		{
			return Equals((InputAnalogActionHandle_t)p);
		}

		public bool Equals(InputAnalogActionHandle_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(InputAnalogActionHandle_t a, InputAnalogActionHandle_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(InputAnalogActionHandle_t a, InputAnalogActionHandle_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(InputAnalogActionHandle_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
