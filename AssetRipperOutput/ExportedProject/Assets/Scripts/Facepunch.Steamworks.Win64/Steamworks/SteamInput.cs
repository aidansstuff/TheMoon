using System.Collections.Generic;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamInput : SteamClientClass<SteamInput>
	{
		internal const int STEAM_CONTROLLER_MAX_COUNT = 16;

		private static readonly InputHandle_t[] queryArray = new InputHandle_t[16];

		internal static Dictionary<string, InputDigitalActionHandle_t> DigitalHandles = new Dictionary<string, InputDigitalActionHandle_t>();

		internal static Dictionary<string, InputAnalogActionHandle_t> AnalogHandles = new Dictionary<string, InputAnalogActionHandle_t>();

		internal static Dictionary<string, InputActionSetHandle_t> ActionSets = new Dictionary<string, InputActionSetHandle_t>();

		internal static ISteamInput Internal => SteamClientClass<SteamInput>.Interface as ISteamInput;

		public static IEnumerable<Controller> Controllers
		{
			get
			{
				int num = Internal.GetConnectedControllers(queryArray);
				for (int i = 0; i < num; i++)
				{
					yield return new Controller(queryArray[i]);
				}
			}
		}

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamInput(server));
		}

		public static void RunFrame()
		{
			Internal.RunFrame();
		}

		public static string GetDigitalActionGlyph(Controller controller, string action)
		{
			InputActionOrigin originsOut = InputActionOrigin.None;
			Internal.GetDigitalActionOrigins(controller.Handle, Internal.GetCurrentActionSet(controller.Handle), GetDigitalActionHandle(action), ref originsOut);
			return Internal.GetGlyphForActionOrigin(originsOut);
		}

		internal static InputDigitalActionHandle_t GetDigitalActionHandle(string name)
		{
			if (DigitalHandles.TryGetValue(name, out var value))
			{
				return value;
			}
			value = Internal.GetDigitalActionHandle(name);
			DigitalHandles.Add(name, value);
			return value;
		}

		internal static InputAnalogActionHandle_t GetAnalogActionHandle(string name)
		{
			if (AnalogHandles.TryGetValue(name, out var value))
			{
				return value;
			}
			value = Internal.GetAnalogActionHandle(name);
			AnalogHandles.Add(name, value);
			return value;
		}

		internal static InputActionSetHandle_t GetActionSetHandle(string name)
		{
			if (ActionSets.TryGetValue(name, out var value))
			{
				return value;
			}
			value = Internal.GetActionSetHandle(name);
			ActionSets.Add(name, value);
			return value;
		}
	}
}
