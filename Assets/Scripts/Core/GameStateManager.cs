using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Assets.Scripts.Core
{
  public class GameStateManager : MonoBehaviour
  {
		public enum ControllerTypes
		{
			DualShock4 = 1,
			DualSence = 2,
			XBox = 3,
			Switch = 4,
			Keyboard = 5,
			SteamDeck = 6,
			XBoxSeries = 7
		}

		private CurrentUserData _currentUserData = new CurrentUserData();
		public static GameStateManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				DontDestroyOnLoad(this);
				Instance = this;
			}
		}

		#region Player/Controller

		public CurrentUserData CurrentUser => _currentUserData;

		public class CurrentUserData
		{
			public bool IsCurrentDevice(InputDevice inputDevice) => inputDevice == CurrentInputDevice;
			public InputDevice CurrentInputDevice { get; set; }

#if UNITY_PS5
			public int UserId => UnityEngine.PS5.Utility.initialUserId;

			public PS5Input.LoggedInUser LoggedInUser
			{
				get
				{
					if (CurrentInputDevice is DualSenseGamepad)
					{
						var slotId = ((DualSenseGamepad)CurrentInputDevice).slotIndex;
						return PS5Input.RefreshUsersDetails(slotId);
					}
					else
					{
						GameLog.Log($"Unknown Device. Name: {CurrentInputDevice.name} , ID: {CurrentInputDevice.deviceId}");
					}
					return new PS5Input.LoggedInUser();
				}
			}

			public void SetLightBarColor(UnityEngine.Color color)
			{
				if (CurrentInputDevice is DualSenseGamepad)
				{
					((DualSenseGamepad)CurrentInputDevice).SetLightBarColor(color);
				}
			}

			public void ResetLightBarColor()
			{
				if (CurrentInputDevice is DualSenseGamepad)
				{
					((DualSenseGamepad)CurrentInputDevice).ResetLightBarColor();
				}
			}
#endif

			public ControllerTypes GetControllerType()
			{
#if UNITY_PS5
				return ControllerTypes.DualSence;
#else
				// Hard coded based on platform, e.g. consoles
				if (Application.platform == RuntimePlatform.PS5)
				{
					return ControllerTypes.DualSence;
				}
				else if (Application.platform == RuntimePlatform.PS4)
				{
					return ControllerTypes.DualShock4;
				}
				else if (Application.platform == RuntimePlatform.Switch)
				{
					return ControllerTypes.Switch;
				}
				else if (Application.platform == RuntimePlatform.GameCoreXboxSeries)
				{
					return ControllerTypes.XBox;
				}

				// Detect based on input device, e.g. pc
				if (CurrentInputDevice is XInputController)
				{
					return ControllerTypes.XBox;
				}
				else if (CurrentInputDevice is DualShockGamepad)
				{
					if (CurrentInputDevice is DualShock4GamepadHID)
					{
						return ControllerTypes.DualShock4;
					}
					else if (CurrentInputDevice is DualSenseGamepadHID)
					{
						return ControllerTypes.DualSence;
					}
				}
				else if (CurrentInputDevice is Keyboard)
				{
					return ControllerTypes.Keyboard;
				}
				return ControllerTypes.Keyboard;
#endif
			}
		}

		#endregion
	}
}
