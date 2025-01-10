using System;

namespace Steamworks.Data
{
	internal struct ControllerDigitalActionHandle_t : IEquatable<ControllerDigitalActionHandle_t>, IComparable<ControllerDigitalActionHandle_t>
	{
		public ulong Value;

		public static implicit operator ControllerDigitalActionHandle_t(ulong value)
		{
			ControllerDigitalActionHandle_t result = default(ControllerDigitalActionHandle_t);
			result.Value = value;
			return result;
		}

		public static implicit operator ulong(ControllerDigitalActionHandle_t value)
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
			return Equals((ControllerDigitalActionHandle_t)p);
		}

		public bool Equals(ControllerDigitalActionHandle_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(ControllerDigitalActionHandle_t a, ControllerDigitalActionHandle_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ControllerDigitalActionHandle_t a, ControllerDigitalActionHandle_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(ControllerDigitalActionHandle_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
