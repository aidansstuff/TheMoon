using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamInput : SteamInterface
	{
		internal ISteamInput(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamInput_v001();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamInput_v001();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_Init")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _Init(IntPtr self);

		internal bool Init()
		{
			return _Init(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_Shutdown")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _Shutdown(IntPtr self);

		internal bool Shutdown()
		{
			return _Shutdown(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_RunFrame")]
		private static extern void _RunFrame(IntPtr self);

		internal void RunFrame()
		{
			_RunFrame(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetConnectedControllers")]
		private static extern int _GetConnectedControllers(IntPtr self, [In][Out] InputHandle_t[] handlesOut);

		internal int GetConnectedControllers([In][Out] InputHandle_t[] handlesOut)
		{
			return _GetConnectedControllers(Self, handlesOut);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetActionSetHandle")]
		private static extern InputActionSetHandle_t _GetActionSetHandle(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszActionSetName);

		internal InputActionSetHandle_t GetActionSetHandle([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszActionSetName)
		{
			return _GetActionSetHandle(Self, pszActionSetName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_ActivateActionSet")]
		private static extern void _ActivateActionSet(IntPtr self, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle);

		internal void ActivateActionSet(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle)
		{
			_ActivateActionSet(Self, inputHandle, actionSetHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetCurrentActionSet")]
		private static extern InputActionSetHandle_t _GetCurrentActionSet(IntPtr self, InputHandle_t inputHandle);

		internal InputActionSetHandle_t GetCurrentActionSet(InputHandle_t inputHandle)
		{
			return _GetCurrentActionSet(Self, inputHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_ActivateActionSetLayer")]
		private static extern void _ActivateActionSetLayer(IntPtr self, InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle);

		internal void ActivateActionSetLayer(InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle)
		{
			_ActivateActionSetLayer(Self, inputHandle, actionSetLayerHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_DeactivateActionSetLayer")]
		private static extern void _DeactivateActionSetLayer(IntPtr self, InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle);

		internal void DeactivateActionSetLayer(InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle)
		{
			_DeactivateActionSetLayer(Self, inputHandle, actionSetLayerHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_DeactivateAllActionSetLayers")]
		private static extern void _DeactivateAllActionSetLayers(IntPtr self, InputHandle_t inputHandle);

		internal void DeactivateAllActionSetLayers(InputHandle_t inputHandle)
		{
			_DeactivateAllActionSetLayers(Self, inputHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetActiveActionSetLayers")]
		private static extern int _GetActiveActionSetLayers(IntPtr self, InputHandle_t inputHandle, [In][Out] InputActionSetHandle_t[] handlesOut);

		internal int GetActiveActionSetLayers(InputHandle_t inputHandle, [In][Out] InputActionSetHandle_t[] handlesOut)
		{
			return _GetActiveActionSetLayers(Self, inputHandle, handlesOut);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetDigitalActionHandle")]
		private static extern InputDigitalActionHandle_t _GetDigitalActionHandle(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszActionName);

		internal InputDigitalActionHandle_t GetDigitalActionHandle([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszActionName)
		{
			return _GetDigitalActionHandle(Self, pszActionName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetDigitalActionData")]
		private static extern DigitalState _GetDigitalActionData(IntPtr self, InputHandle_t inputHandle, InputDigitalActionHandle_t digitalActionHandle);

		internal DigitalState GetDigitalActionData(InputHandle_t inputHandle, InputDigitalActionHandle_t digitalActionHandle)
		{
			return _GetDigitalActionData(Self, inputHandle, digitalActionHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetDigitalActionOrigins")]
		private static extern int _GetDigitalActionOrigins(IntPtr self, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputDigitalActionHandle_t digitalActionHandle, ref InputActionOrigin originsOut);

		internal int GetDigitalActionOrigins(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputDigitalActionHandle_t digitalActionHandle, ref InputActionOrigin originsOut)
		{
			return _GetDigitalActionOrigins(Self, inputHandle, actionSetHandle, digitalActionHandle, ref originsOut);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetAnalogActionHandle")]
		private static extern InputAnalogActionHandle_t _GetAnalogActionHandle(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszActionName);

		internal InputAnalogActionHandle_t GetAnalogActionHandle([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszActionName)
		{
			return _GetAnalogActionHandle(Self, pszActionName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetAnalogActionData")]
		private static extern AnalogState _GetAnalogActionData(IntPtr self, InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle);

		internal AnalogState GetAnalogActionData(InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle)
		{
			return _GetAnalogActionData(Self, inputHandle, analogActionHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetAnalogActionOrigins")]
		private static extern int _GetAnalogActionOrigins(IntPtr self, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputAnalogActionHandle_t analogActionHandle, ref InputActionOrigin originsOut);

		internal int GetAnalogActionOrigins(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputAnalogActionHandle_t analogActionHandle, ref InputActionOrigin originsOut)
		{
			return _GetAnalogActionOrigins(Self, inputHandle, actionSetHandle, analogActionHandle, ref originsOut);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetGlyphForActionOrigin")]
		private static extern Utf8StringPointer _GetGlyphForActionOrigin(IntPtr self, InputActionOrigin eOrigin);

		internal string GetGlyphForActionOrigin(InputActionOrigin eOrigin)
		{
			Utf8StringPointer utf8StringPointer = _GetGlyphForActionOrigin(Self, eOrigin);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetStringForActionOrigin")]
		private static extern Utf8StringPointer _GetStringForActionOrigin(IntPtr self, InputActionOrigin eOrigin);

		internal string GetStringForActionOrigin(InputActionOrigin eOrigin)
		{
			Utf8StringPointer utf8StringPointer = _GetStringForActionOrigin(Self, eOrigin);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_StopAnalogActionMomentum")]
		private static extern void _StopAnalogActionMomentum(IntPtr self, InputHandle_t inputHandle, InputAnalogActionHandle_t eAction);

		internal void StopAnalogActionMomentum(InputHandle_t inputHandle, InputAnalogActionHandle_t eAction)
		{
			_StopAnalogActionMomentum(Self, inputHandle, eAction);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetMotionData")]
		private static extern MotionState _GetMotionData(IntPtr self, InputHandle_t inputHandle);

		internal MotionState GetMotionData(InputHandle_t inputHandle)
		{
			return _GetMotionData(Self, inputHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_TriggerVibration")]
		private static extern void _TriggerVibration(IntPtr self, InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed);

		internal void TriggerVibration(InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed)
		{
			_TriggerVibration(Self, inputHandle, usLeftSpeed, usRightSpeed);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_SetLEDColor")]
		private static extern void _SetLEDColor(IntPtr self, InputHandle_t inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags);

		internal void SetLEDColor(InputHandle_t inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags)
		{
			_SetLEDColor(Self, inputHandle, nColorR, nColorG, nColorB, nFlags);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_TriggerHapticPulse")]
		private static extern void _TriggerHapticPulse(IntPtr self, InputHandle_t inputHandle, SteamControllerPad eTargetPad, ushort usDurationMicroSec);

		internal void TriggerHapticPulse(InputHandle_t inputHandle, SteamControllerPad eTargetPad, ushort usDurationMicroSec)
		{
			_TriggerHapticPulse(Self, inputHandle, eTargetPad, usDurationMicroSec);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_TriggerRepeatedHapticPulse")]
		private static extern void _TriggerRepeatedHapticPulse(IntPtr self, InputHandle_t inputHandle, SteamControllerPad eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags);

		internal void TriggerRepeatedHapticPulse(InputHandle_t inputHandle, SteamControllerPad eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags)
		{
			_TriggerRepeatedHapticPulse(Self, inputHandle, eTargetPad, usDurationMicroSec, usOffMicroSec, unRepeat, nFlags);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_ShowBindingPanel")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ShowBindingPanel(IntPtr self, InputHandle_t inputHandle);

		internal bool ShowBindingPanel(InputHandle_t inputHandle)
		{
			return _ShowBindingPanel(Self, inputHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetInputTypeForHandle")]
		private static extern InputType _GetInputTypeForHandle(IntPtr self, InputHandle_t inputHandle);

		internal InputType GetInputTypeForHandle(InputHandle_t inputHandle)
		{
			return _GetInputTypeForHandle(Self, inputHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetControllerForGamepadIndex")]
		private static extern InputHandle_t _GetControllerForGamepadIndex(IntPtr self, int nIndex);

		internal InputHandle_t GetControllerForGamepadIndex(int nIndex)
		{
			return _GetControllerForGamepadIndex(Self, nIndex);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetGamepadIndexForController")]
		private static extern int _GetGamepadIndexForController(IntPtr self, InputHandle_t ulinputHandle);

		internal int GetGamepadIndexForController(InputHandle_t ulinputHandle)
		{
			return _GetGamepadIndexForController(Self, ulinputHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetStringForXboxOrigin")]
		private static extern Utf8StringPointer _GetStringForXboxOrigin(IntPtr self, XboxOrigin eOrigin);

		internal string GetStringForXboxOrigin(XboxOrigin eOrigin)
		{
			Utf8StringPointer utf8StringPointer = _GetStringForXboxOrigin(Self, eOrigin);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetGlyphForXboxOrigin")]
		private static extern Utf8StringPointer _GetGlyphForXboxOrigin(IntPtr self, XboxOrigin eOrigin);

		internal string GetGlyphForXboxOrigin(XboxOrigin eOrigin)
		{
			Utf8StringPointer utf8StringPointer = _GetGlyphForXboxOrigin(Self, eOrigin);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetActionOriginFromXboxOrigin")]
		private static extern InputActionOrigin _GetActionOriginFromXboxOrigin(IntPtr self, InputHandle_t inputHandle, XboxOrigin eOrigin);

		internal InputActionOrigin GetActionOriginFromXboxOrigin(InputHandle_t inputHandle, XboxOrigin eOrigin)
		{
			return _GetActionOriginFromXboxOrigin(Self, inputHandle, eOrigin);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_TranslateActionOrigin")]
		private static extern InputActionOrigin _TranslateActionOrigin(IntPtr self, InputType eDestinationInputType, InputActionOrigin eSourceOrigin);

		internal InputActionOrigin TranslateActionOrigin(InputType eDestinationInputType, InputActionOrigin eSourceOrigin)
		{
			return _TranslateActionOrigin(Self, eDestinationInputType, eSourceOrigin);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetDeviceBindingRevision")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetDeviceBindingRevision(IntPtr self, InputHandle_t inputHandle, ref int pMajor, ref int pMinor);

		internal bool GetDeviceBindingRevision(InputHandle_t inputHandle, ref int pMajor, ref int pMinor)
		{
			return _GetDeviceBindingRevision(Self, inputHandle, ref pMajor, ref pMinor);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamInput_GetRemotePlaySessionID")]
		private static extern uint _GetRemotePlaySessionID(IntPtr self, InputHandle_t inputHandle);

		internal uint GetRemotePlaySessionID(InputHandle_t inputHandle)
		{
			return _GetRemotePlaySessionID(Self, inputHandle);
		}
	}
}
