using System;

using UnityEngine;

namespace ActionInput
{
	public enum Button
	{
		None,
		FaceBottom, FaceRight, FaceLeft, FaceTop,
		LeftShoulder, RightShoulder,
		LeftSpecial, RightSpecial,
		LeftStick, RightStick,

		// Virtual buttons
		LeftStickDown, LeftStickRight, LeftStickLeft, LeftStickUp,
		RightStickDown, RightStickRight, RightStickLeft, RightStickUp,
		DpadDown, DpadRight, DpadLeft, DpadUp,
		LeftTrigger, RightTrigger,
	}

	public enum Axis
	{
		None,
		MouseX, MouseY,
		LeftStickX, LeftStickY,
		RightStickX, RightStickY,
		DpadX, DpadY,
		LeftTrigger, RightTrigger,
	}

	public struct ActionMap
	{
		public KeyCode[] keys;
		public Button[] buttons;

		public ActionMap(KeyCode[] keys, Button[] buttons)
		{
			this.keys = keys;
			this.buttons = buttons;
		}

		public ActionMap(KeyCode key, Button button)
		{
			keys = new KeyCode[1] { key };
			buttons = new Button[1] { button };
		}

		public bool GetHeld()
		{
			if (keys != null)
				for (int i = 0; i < keys.Length; i++)
					if (Input.GetKey(keys[i]))
						return true;

			if (buttons != null)
				for (int i = 0; i < buttons.Length; i++)
					if (ActionInput.GetButtonHeld(buttons[i]))
						return true;

			return false;
		}

		public bool GetDown()
		{
			if (keys != null)
				for (int i = 0; i < keys.Length; i++)
					if (Input.GetKeyDown(keys[i]))
						return true;

			if (buttons != null)
				for (int i = 0; i < buttons.Length; i++)
					if (ActionInput.GetButtonDown(buttons[i]))
						return true;

			return false;
		}

		public bool GetUp()
		{
			if (keys != null)
				for (int i = 0; i < keys.Length; i++)
					if (Input.GetKeyUp(keys[i]))
						return true;

			if (buttons != null)
				for (int i = 0; i < buttons.Length; i++)
					if (ActionInput.GetButtonUp(buttons[i]))
						return true;

			return false;
		}
	}

	public struct AxisMap
	{
		public AxisConfig[] axisConfigs;

		public AxisMap(params AxisConfig[] axisConfigs)
		{
			this.axisConfigs = axisConfigs;
		}

		public float GetAxis()
		{
			if (axisConfigs == null)
				return 0f;

			float keyButton = 0f;
			float axis = 0f;

			for (int i = 0; i < axisConfigs.Length; i++)
			{
				if (axisConfigs[i].key != KeyCode.None) keyButton += Convert.ToInt32(Input.GetKey(axisConfigs[i].key)) * axisConfigs[i].scale;

				if (axisConfigs[i].button != Button.None) keyButton += Convert.ToInt32(ActionInput.GetButtonHeld(axisConfigs[i].button)) * axisConfigs[i].scale;

				if (axisConfigs[i].axis != Axis.None)
				{
					float temp = Input.GetAxisRaw(axisConfigs[i].axis.GetName()) * axisConfigs[i].scale;

					if (Mathf.Abs(temp) > Mathf.Abs(axis)) axis = temp;
				}
			}

			keyButton = Mathf.Clamp(keyButton, -1f, 1f);

			return Mathf.Abs(keyButton) > Mathf.Abs(axis) ? keyButton : axis;
		}
	}

	public struct AxisConfig
	{
		public KeyCode key;
		public Button button;
		public Axis axis;
		public float scale;

		public AxisConfig(KeyCode key, float scale)
		{
			this.key = key;
			button = Button.None;
			axis = Axis.None;
			this.scale = scale;
		}

		public AxisConfig(Button button, float scale)
		{
			key = KeyCode.None;
			this.button = button;
			axis = Axis.None;
			this.scale = scale;
		}

		public AxisConfig(Axis axis, float scale)
		{
			key = KeyCode.None;
			button = Button.None;
			this.axis = axis;
			this.scale = scale;
		}
	}

	static class Extensions
	{
		// BUTTON
		public static KeyCode GetKeyCode(this Button button)
		{
			switch (button)
			{
				case Button.FaceBottom:
					return KeyCode.Joystick1Button0;

				case Button.FaceRight:
					return KeyCode.Joystick1Button1;

				case Button.FaceLeft:
					return KeyCode.Joystick1Button2;

				case Button.FaceTop:
					return KeyCode.Joystick1Button3;

				case Button.LeftShoulder:
					return KeyCode.Joystick1Button4;

				case Button.RightShoulder:
					return KeyCode.Joystick1Button5;

				case Button.LeftSpecial:
					return KeyCode.Joystick1Button6;

				case Button.RightSpecial:
					return KeyCode.Joystick1Button7;

				case Button.LeftStick:
					return KeyCode.Joystick1Button8;

				case Button.RightStick:
					return KeyCode.Joystick1Button9;

				default:
					return KeyCode.None;
			}
		}

		public static Axis GetAxis(this Button button)
		{
			switch (button)
			{
				case Button.LeftStickDown:
				case Button.LeftStickUp:
					return Axis.LeftStickY;

				case Button.LeftStickRight:
				case Button.LeftStickLeft:
					return Axis.LeftStickX;

				case Button.RightStickDown:
				case Button.RightStickUp:
					return Axis.RightStickY;

				case Button.RightStickRight:
				case Button.RightStickLeft:
					return Axis.RightStickX;

				case Button.DpadDown:
				case Button.DpadUp:
					return Axis.DpadY;

				case Button.DpadRight:
				case Button.DpadLeft:
					return Axis.DpadX;

				case Button.LeftTrigger:
					return Axis.LeftTrigger;

				case Button.RightTrigger:
					return Axis.RightTrigger;

				default:
					return Axis.None;
			}
		}

		public static bool GetIsReversed(this Button button)
		{
			switch (button)
			{
				case Button.LeftStickDown:
				case Button.LeftStickLeft:
				case Button.RightStickDown:
				case Button.RightStickLeft:
				case Button.DpadDown:
				case Button.DpadLeft:
					return true;

				default:
					return false;
			}
		}

		// AXIS
		public static String GetName(this Axis axis)
		{
			switch (axis)
			{
				case Axis.MouseX:
					return "Mouse X";

				case Axis.MouseY:
					return "Mouse Y";

				case Axis.LeftStickX:
					return "Joystick1 LeftStick X";

				case Axis.LeftStickY:
					return "Joystick1 LeftStick Y";

				case Axis.RightStickX:
					return "Joystick1 RightStick X";

				case Axis.RightStickY:
					return "Joystick1 RightStick Y";

				case Axis.DpadX:
					return "Joystick1 Dpad X";

				case Axis.DpadY:
					return "Joystick1 Dpad Y";

				case Axis.LeftTrigger:
					return "Joystick1 LeftTrigger";

				case Axis.RightTrigger:
					return "Joystick1 RightTrigger";

				default:
					return string.Empty;
			}
		}
	}

	public abstract class ActionInput : Singleton<ActionInput>
	{
		private float lastMouseX, lastMouseY, lastLeftTrigger, lastRightTrigger, lastDpadY, lastDpadX, lastLeftStickY, lastLeftStickX, lastRightStickY, lastRightStickX;

		private void LateUpdate()
		{
			lastMouseX = Input.GetAxisRaw(Axis.MouseX.GetName());
			lastMouseY = Input.GetAxisRaw(Axis.MouseY.GetName());
			lastLeftTrigger = Input.GetAxisRaw(Axis.LeftTrigger.GetName());
			lastRightTrigger = Input.GetAxisRaw(Axis.RightTrigger.GetName());
			lastDpadY = Input.GetAxisRaw(Axis.DpadY.GetName());
			lastDpadX = Input.GetAxisRaw(Axis.DpadX.GetName());
			lastLeftStickY = Input.GetAxisRaw(Axis.LeftStickY.GetName());
			lastLeftStickX = Input.GetAxisRaw(Axis.LeftStickX.GetName());
			lastRightStickY = Input.GetAxisRaw(Axis.RightStickY.GetName());
			lastRightStickX = Input.GetAxisRaw(Axis.RightStickX.GetName());
		}

		public float GetLastAxisValue(Axis axis)
		{
			switch (axis)
			{
				case Axis.MouseX:
					return lastMouseX;

				case Axis.MouseY:
					return lastMouseY;

				case Axis.LeftTrigger:
					return lastLeftTrigger;

				case Axis.RightTrigger:
					return lastRightTrigger;

				case Axis.DpadY:
					return lastDpadY;

				case Axis.DpadX:
					return lastDpadX;

				case Axis.LeftStickY:
					return lastLeftStickY;

				case Axis.LeftStickX:
					return lastLeftStickX;

				case Axis.RightStickY:
					return lastRightStickY;

				case Axis.RightStickX:
					return lastRightStickX;

				default:
					return 0f;
			}
		}

		public static bool GetButtonHeld(Button button)
		{
			KeyCode buttonKeyCode = button.GetKeyCode();

			if (button.GetKeyCode() != KeyCode.None)
			{
				return Input.GetKeyDown(buttonKeyCode);
			}
			else // Virtual button
			{
				return button.GetIsReversed() ?
					(Input.GetAxisRaw(button.GetAxis().GetName()) < 0f) : (Input.GetAxisRaw(button.GetAxis().GetName()) > 0f);
			}
		}

		public static bool GetButtonDown(Button button)
		{
			KeyCode buttonKeyCode = button.GetKeyCode();

			if (buttonKeyCode != KeyCode.None)
			{
				return Input.GetKeyDown(buttonKeyCode);
			}
			else // Virtual button
			{
				Axis buttonAxis = button.GetAxis();

				return button.GetIsReversed() ?
					(instance.GetLastAxisValue(buttonAxis) >= 0f && Input.GetAxisRaw(buttonAxis.GetName()) < 0f) : (instance.GetLastAxisValue(buttonAxis) <= 0f && Input.GetAxisRaw(buttonAxis.GetName()) > 0f);
			}
		}

		public static bool GetButtonUp(Button button)
		{
			KeyCode buttonKeyCode = button.GetKeyCode();

			if (buttonKeyCode != KeyCode.None)
			{
				return Input.GetKeyUp(buttonKeyCode);
			}
			else // Virtual button
			{
				Axis buttonAxis = button.GetAxis();

				return button.GetIsReversed() ?
					(instance.GetLastAxisValue(buttonAxis) < 0f && Input.GetAxisRaw(buttonAxis.GetName()) >= 0f) : (instance.GetLastAxisValue(buttonAxis) > 0f && Input.GetAxisRaw(buttonAxis.GetName()) <= 0f);
			}
		}
	}
}
