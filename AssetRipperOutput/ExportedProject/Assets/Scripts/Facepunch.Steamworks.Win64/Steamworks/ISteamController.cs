using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamController : SteamInterface
	{
		internal ISteamController(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamController_v007();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamController_v007();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_Init")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _Init(IntPtr self);

		internal bool Init()
		{
			return _Init(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_Shutdown")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _Shutdown(IntPtr self);

		internal bool Shutdown()
		{
			return _Shutdown(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_RunFrame")]
		private static extern void _RunFrame(IntPtr self);

		internal void RunFrame()
		{
			_RunFrame(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetConnectedControllers")]
		private static extern int _GetConnectedControllers(IntPtr self, [In][Out] ControllerHandle_t[] handlesOut);

		internal int GetConnectedControllers([In][Out] ControllerHandle_t[] handlesOut)
		{
			return _GetConnectedControllers(Self, handlesOut);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetActionSetHandle")]
		private static extern ControllerActionSetHandle_t _GetActionSetHandle(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszActionSetName);

		internal ControllerActionSetHandle_t GetActionSetHandle([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszActionSetName)
		{
			return _GetActionSetHandle(Self, pszActionSetName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_ActivateActionSet")]
		private static extern void _ActivateActionSet(IntPtr self, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle);

		internal void ActivateActionSet(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle)
		{
			_ActivateActionSet(Self, controllerHandle, actionSetHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetCurrentActionSet")]
		private static extern ControllerActionSetHandle_t _GetCurrentActionSet(IntPtr self, ControllerHandle_t controllerHandle);

		internal ControllerActionSetHandle_t GetCurrentActionSet(ControllerHandle_t controllerHandle)
		{
			return _GetCurrentActionSet(Self, controllerHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_ActivateActionSetLayer")]
		private static extern void _ActivateActionSetLayer(IntPtr self, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle);

		internal void ActivateActionSetLayer(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle)
		{
			_ActivateActionSetLayer(Self, controllerHandle, actionSetLayerHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_DeactivateActionSetLayer")]
		private static extern void _DeactivateActionSetLayer(IntPtr self, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle);

		internal void DeactivateActionSetLayer(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle)
		{
			_DeactivateActionSetLayer(Self, controllerHandle, actionSetLayerHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_DeactivateAllActionSetLayers")]
		private static extern void _DeactivateAllActionSetLayers(IntPtr self, ControllerHandle_t controllerHandle);

		internal void DeactivateAllActionSetLayers(ControllerHandle_t controllerHandle)
		{
			_DeactivateAllActionSetLayers(Self, controllerHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetActiveActionSetLayers")]
		private static extern int _GetActiveActionSetLayers(IntPtr self, ControllerHandle_t controllerHandle, [In][Out] ControllerActionSetHandle_t[] handlesOut);

		internal int GetActiveActionSetLayers(ControllerHandle_t controllerHandle, [In][Out] ControllerActionSetHandle_t[] handlesOut)
		{
			return _GetActiveActionSetLayers(Self, controllerHandle, handlesOut);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetDigitalActionHandle")]
		private static extern ControllerDigitalActionHandle_t _GetDigitalActionHandle(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszActionName);

		internal ControllerDigitalActionHandle_t GetDigitalActionHandle([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszActionName)
		{
			return _GetDigitalActionHandle(Self, pszActionName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetDigitalActionData")]
		private static extern DigitalState _GetDigitalActionData(IntPtr self, ControllerHandle_t controllerHandle, ControllerDigitalActionHandle_t digitalActionHandle);

		internal DigitalState GetDigitalActionData(ControllerHandle_t controllerHandle, ControllerDigitalActionHandle_t digitalActionHandle)
		{
			return _GetDigitalActionData(Self, controllerHandle, digitalActionHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetDigitalActionOrigins")]
		private static extern int _GetDigitalActionOrigins(IntPtr self, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerDigitalActionHandle_t digitalActionHandle, ref ControllerActionOrigin originsOut);

		internal int GetDigitalActionOrigins(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerDigitalActionHandle_t digitalActionHandle, ref ControllerActionOrigin originsOut)
		{
			return _GetDigitalActionOrigins(Self, controllerHandle, actionSetHandle, digitalActionHandle, ref originsOut);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetAnalogActionHandle")]
		private static extern ControllerAnalogActionHandle_t _GetAnalogActionHandle(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszActionName);

		internal ControllerAnalogActionHandle_t GetAnalogActionHandle([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszActionName)
		{
			return _GetAnalogActionHandle(Self, pszActionName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetAnalogActionData")]
		private static extern AnalogState _GetAnalogActionData(IntPtr self, ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t analogActionHandle);

		internal AnalogState GetAnalogActionData(ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t analogActionHandle)
		{
			return _GetAnalogActionData(Self, controllerHandle, analogActionHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetAnalogActionOrigins")]
		private static extern int _GetAnalogActionOrigins(IntPtr self, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerAnalogActionHandle_t analogActionHandle, ref ControllerActionOrigin originsOut);

		internal int GetAnalogActionOrigins(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerAnalogActionHandle_t analogActionHandle, ref ControllerActionOrigin originsOut)
		{
			return _GetAnalogActionOrigins(Self, controllerHandle, actionSetHandle, analogActionHandle, ref originsOut);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetGlyphForActionOrigin")]
		private static extern Utf8StringPointer _GetGlyphForActionOrigin(IntPtr self, ControllerActionOrigin eOrigin);

		internal string GetGlyphForActionOrigin(ControllerActionOrigin eOrigin)
		{
			Utf8StringPointer utf8StringPointer = _GetGlyphForActionOrigin(Self, eOrigin);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetStringForActionOrigin")]
		private static extern Utf8StringPointer _GetStringForActionOrigin(IntPtr self, ControllerActionOrigin eOrigin);

		internal string GetStringForActionOrigin(ControllerActionOrigin eOrigin)
		{
			Utf8StringPointer utf8StringPointer = _GetStringForActionOrigin(Self, eOrigin);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_StopAnalogActionMomentum")]
		private static extern void _StopAnalogActionMomentum(IntPtr self, ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t eAction);

		internal void StopAnalogActionMomentum(ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t eAction)
		{
			_StopAnalogActionMomentum(Self, controllerHandle, eAction);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetMotionData")]
		private static extern MotionState _GetMotionData(IntPtr self, ControllerHandle_t controllerHandle);

		internal MotionState GetMotionData(ControllerHandle_t controllerHandle)
		{
			return _GetMotionData(Self, controllerHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_TriggerHapticPulse")]
		private static extern void _TriggerHapticPulse(IntPtr self, ControllerHandle_t controllerHandle, SteamControllerPad eTargetPad, ushort usDurationMicroSec);

		internal void TriggerHapticPulse(ControllerHandle_t controllerHandle, SteamControllerPad eTargetPad, ushort usDurationMicroSec)
		{
			_TriggerHapticPulse(Self, controllerHandle, eTargetPad, usDurationMicroSec);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_TriggerRepeatedHapticPulse")]
		private static extern void _TriggerRepeatedHapticPulse(IntPtr self, ControllerHandle_t controllerHandle, SteamControllerPad eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags);

		internal void TriggerRepeatedHapticPulse(ControllerHandle_t controllerHandle, SteamControllerPad eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags)
		{
			_TriggerRepeatedHapticPulse(Self, controllerHandle, eTargetPad, usDurationMicroSec, usOffMicroSec, unRepeat, nFlags);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_TriggerVibration")]
		private static extern void _TriggerVibration(IntPtr self, ControllerHandle_t controllerHandle, ushort usLeftSpeed, ushort usRightSpeed);

		internal void TriggerVibration(ControllerHandle_t controllerHandle, ushort usLeftSpeed, ushort usRightSpeed)
		{
			_TriggerVibration(Self, controllerHandle, usLeftSpeed, usRightSpeed);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_SetLEDColor")]
		private static extern void _SetLEDColor(IntPtr self, ControllerHandle_t controllerHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags);

		internal void SetLEDColor(ControllerHandle_t controllerHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags)
		{
			_SetLEDColor(Self, controllerHandle, nColorR, nColorG, nColorB, nFlags);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_ShowBindingPanel")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ShowBindingPanel(IntPtr self, ControllerHandle_t controllerHandle);

		internal bool ShowBindingPanel(ControllerHandle_t controllerHandle)
		{
			return _ShowBindingPanel(Self, controllerHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetInputTypeForHandle")]
		private static extern InputType _GetInputTypeForHandle(IntPtr self, ControllerHandle_t controllerHandle);

		internal InputType GetInputTypeForHandle(ControllerHandle_t controllerHandle)
		{
			return _GetInputTypeForHandle(Self, controllerHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetControllerForGamepadIndex")]
		private static extern ControllerHandle_t _GetControllerForGamepadIndex(IntPtr self, int nIndex);

		internal ControllerHandle_t GetControllerForGamepadIndex(int nIndex)
		{
			return _GetControllerForGamepadIndex(Self, nIndex);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetGamepadIndexForController")]
		private static extern int _GetGamepadIndexForController(IntPtr self, ControllerHandle_t ulControllerHandle);

		internal int GetGamepadIndexForController(ControllerHandle_t ulControllerHandle)
		{
			return _GetGamepadIndexForController(Self, ulControllerHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetStringForXboxOrigin")]
		private static extern Utf8StringPointer _GetStringForXboxOrigin(IntPtr self, XboxOrigin eOrigin);

		internal string GetStringForXboxOrigin(XboxOrigin eOrigin)
		{
			Utf8StringPointer utf8StringPointer = _GetStringForXboxOrigin(Self, eOrigin);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetGlyphForXboxOrigin")]
		private static extern Utf8StringPointer _GetGlyphForXboxOrigin(IntPtr self, XboxOrigin eOrigin);

		internal string GetGlyphForXboxOrigin(XboxOrigin eOrigin)
		{
			Utf8StringPointer utf8StringPointer = _GetGlyphForXboxOrigin(Self, eOrigin);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetActionOriginFromXboxOrigin")]
		private static extern ControllerActionOrigin _GetActionOriginFromXboxOrigin(IntPtr self, ControllerHandle_t controllerHandle, XboxOrigin eOrigin);

		internal ControllerActionOrigin GetActionOriginFromXboxOrigin(ControllerHandle_t controllerHandle, XboxOrigin eOrigin)
		{
			return _GetActionOriginFromXboxOrigin(Self, controllerHandle, eOrigin);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_TranslateActionOrigin")]
		private static extern ControllerActionOrigin _TranslateActionOrigin(IntPtr self, InputType eDestinationInputType, ControllerActionOrigin eSourceOrigin);

		internal ControllerActionOrigin TranslateActionOrigin(InputType eDestinationInputType, ControllerActionOrigin eSourceOrigin)
		{
			return _TranslateActionOrigin(Self, eDestinationInputType, eSourceOrigin);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamController_GetControllerBindingRevision")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetControllerBindingRevision(IntPtr self, ControllerHandle_t controllerHandle, ref int pMajor, ref int pMinor);

		internal bool GetControllerBindingRevision(ControllerHandle_t controllerHandle, ref int pMajor, ref int pMinor)
		{
			return _GetControllerBindingRevision(Self, controllerHandle, ref pMajor, ref pMinor);
		}
	}
}
