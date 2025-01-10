using Steamworks.Data;

namespace Steamworks
{
	public struct Controller
	{
		internal InputHandle_t Handle;

		public ulong Id => Handle.Value;

		public InputType InputType => SteamInput.Internal.GetInputTypeForHandle(Handle);

		public string ActionSet
		{
			set
			{
				SteamInput.Internal.ActivateActionSet(Handle, SteamInput.Internal.GetActionSetHandle(value));
			}
		}

		internal Controller(InputHandle_t inputHandle_t)
		{
			Handle = inputHandle_t;
		}

		public void DeactivateLayer(string layer)
		{
			SteamInput.Internal.DeactivateActionSetLayer(Handle, SteamInput.Internal.GetActionSetHandle(layer));
		}

		public void ActivateLayer(string layer)
		{
			SteamInput.Internal.ActivateActionSetLayer(Handle, SteamInput.Internal.GetActionSetHandle(layer));
		}

		public void ClearLayers()
		{
			SteamInput.Internal.DeactivateAllActionSetLayers(Handle);
		}

		public DigitalState GetDigitalState(string actionName)
		{
			return SteamInput.Internal.GetDigitalActionData(Handle, SteamInput.GetDigitalActionHandle(actionName));
		}

		public AnalogState GetAnalogState(string actionName)
		{
			return SteamInput.Internal.GetAnalogActionData(Handle, SteamInput.GetAnalogActionHandle(actionName));
		}

		public override string ToString()
		{
			return $"{InputType}.{Handle.Value}";
		}

		public static bool operator ==(Controller a, Controller b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Controller a, Controller b)
		{
			return !(a == b);
		}

		public override bool Equals(object p)
		{
			return Equals((Controller)p);
		}

		public override int GetHashCode()
		{
			return Handle.GetHashCode();
		}

		public bool Equals(Controller p)
		{
			return p.Handle == Handle;
		}
	}
}
