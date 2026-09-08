using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Quasar.Global;

namespace Quasar.Input;

public class Gamepad
{
	public enum DeadZoneTypes
	{
		Axis,
		Circular
	}

	public struct DeadZoneSetting
	{
		public DeadZoneTypes Type;

		public float Amount;

		public float TriggerAmount;

		public DeadZoneSetting(DeadZoneTypes type, float amount, float triggerAmount)
		{
			Type = type;
			Amount = amount;
			TriggerAmount = triggerAmount;
		}

		public DeadZoneSetting(float amount)
		{
			Type = DeadZoneTypes.Axis;
			Amount = amount;
			TriggerAmount = amount;
		}

		public void GetStickValue(ref Vector2 value, out Vector2 result)
		{
			switch (Type)
			{
			default:
			{
				int num2 = Math.Sign(value.X);
				int num3 = Math.Sign(value.Y);
				result = Vector2.Min(new Vector2(1f, 1f), new Vector2(Math.Max(0f, Math.Abs(value.X) - Amount) / (1f - Amount), Math.Max(0f, Math.Abs(value.Y) - Amount) / (1f - Amount)));
				result = new Vector2(result.X * (float)num2, result.Y * (float)num3);
				break;
			}
			case DeadZoneTypes.Circular:
			{
				float num = value.Length();
				num = Math.Min(1f, Math.Max(0f, num - Amount) / (1f - Amount));
				GameMath.VectorSetLength(ref value, num, out result);
				break;
			}
			}
		}

		public float GetTriggerValue(float value)
		{
			return Math.Max(0f, value - TriggerAmount) / (1f - TriggerAmount);
		}
	}

	private static Gamepad[] gamepads = new Gamepad[4];

	private GamePadState previousState;

	private GamePadState currentState;

	private PlayerIndex index;

	private DeadZoneSetting deadZone = new DeadZoneSetting(DeadZoneTypes.Axis, 0.15f, 0.15f);

	private Stack<DeadZoneSetting> deadZoneStack = new Stack<DeadZoneSetting>(4);

	private static bool vibrationEnabled = true;

	private long vibrateEnd;

	private Vector2 vibrateSpeed = Vector2.Zero;

	public PlayerIndex Index => index;

	public DeadZoneSetting DeadZone
	{
		get
		{
			if (deadZoneStack.Count > 0)
			{
				return deadZoneStack.Peek();
			}
			return deadZone;
		}
		set
		{
			deadZone = value;
		}
	}

	public static bool VibrationEnabled
	{
		get
		{
			return vibrationEnabled;
		}
		set
		{
			vibrationEnabled = value;
		}
	}

	public bool Connected => true;

	public static int ConnectedPads
	{
		get
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				num += (Instance((PlayerIndex)i).Connected ? 1 : 0);
			}
			return num;
		}
	}

	public GamePadState CurrentState => currentState;

	public GamePadState PreviousState => previousState;

	public bool HasTimedVibration => Timer.DefaultTimer.TotalTime <= vibrateEnd;

	public static void SetDeadZone(DeadZoneSetting value)
	{
		Instance(PlayerIndex.One).DeadZone = value;
		Instance(PlayerIndex.Two).DeadZone = value;
		Instance(PlayerIndex.Three).DeadZone = value;
		Instance(PlayerIndex.Four).DeadZone = value;
	}

	public void PushDeadZone(DeadZoneSetting setting)
	{
		deadZoneStack.Push(setting);
	}

	public static void PushDeadZone(List<PlayerIndex> indices, DeadZoneSetting setting)
	{
		foreach (PlayerIndex index in indices)
		{
			Instance(index).PushDeadZone(setting);
		}
	}

	public static void PopDeadZone(List<PlayerIndex> indices)
	{
		foreach (PlayerIndex index in indices)
		{
			Instance(index).PopDeadZone();
		}
	}

	public void PopDeadZone()
	{
		deadZoneStack.Pop();
	}

	private Gamepad(PlayerIndex index)
	{
		this.index = index;
		UpdateControllerStatus();
	}

	public static Gamepad Instance(PlayerIndex index)
	{
		if (gamepads[(int)index] == null)
		{
			gamepads[(int)index] = new Gamepad(index);
		}
		return gamepads[(int)index];
	}

	public static void UpdateStatus()
	{
		Gamepad[] array = gamepads;
		for (int i = 0; i < array.Length; i++)
		{
			array[i]?.UpdateControllerStatus();
		}
	}

	private void UpdateControllerStatus()
	{
		GamePadState state = GamePad.GetState(index, GamePadDeadZone.None);
		if (!state.IsConnected || GamePad.GetCapabilities(index).GamePadType != GamePadType.Guitar)
		{
			previousState = currentState;
			currentState = state;
			CheckTimedVibration();
		}
	}

	public bool PreviousButtonStatus(Buttons buttons)
	{
		return previousState.IsButtonDown(buttons);
	}

	public bool ButtonStatus(Buttons buttons)
	{
		return currentState.IsButtonDown(buttons);
	}

	public static bool AnyButtonStatus(Buttons buttons)
	{
		if (!Instance(PlayerIndex.One).ButtonStatus(buttons) && !Instance(PlayerIndex.Two).ButtonStatus(buttons) && !Instance(PlayerIndex.Three).ButtonStatus(buttons))
		{
			return Instance(PlayerIndex.Four).ButtonStatus(buttons);
		}
		return true;
	}

	public static bool ButtonStatus(List<PlayerIndex> indices, Buttons buttons)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).ButtonStatus(buttons))
			{
				return true;
			}
		}
		return false;
	}

	public static bool AnyButtonStatus(Buttons button, ref PlayerIndex whoPressed)
	{
		for (int i = 0; i < 4; i++)
		{
			if (Instance((PlayerIndex)i).ButtonStatus(button))
			{
				whoPressed = (PlayerIndex)i;
				return true;
			}
		}
		return false;
	}

	public static bool ButtonStatus(List<PlayerIndex> indices, Buttons button, ref PlayerIndex whoPressed)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).ButtonStatus(button))
			{
				whoPressed = index;
				return true;
			}
		}
		return false;
	}

	public bool ButtonPressed(Buttons button)
	{
		if (ButtonStatus(button))
		{
			return !PreviousButtonStatus(button);
		}
		return false;
	}

	public bool ButtonStatus()
	{
		if (!currentState.IsButtonDown(Buttons.A) && !currentState.IsButtonDown(Buttons.B) && !currentState.IsButtonDown(Buttons.X) && !currentState.IsButtonDown(Buttons.Y) && !currentState.IsButtonDown(Buttons.Start) && !currentState.IsButtonDown(Buttons.Back) && !currentState.IsButtonDown(Buttons.LeftStick) && !currentState.IsButtonDown(Buttons.RightStick) && !currentState.IsButtonDown(Buttons.LeftShoulder) && !currentState.IsButtonDown(Buttons.RightShoulder) && !currentState.IsButtonDown(Buttons.DPadDown) && !currentState.IsButtonDown(Buttons.DPadLeft) && !currentState.IsButtonDown(Buttons.DPadRight))
		{
			return currentState.IsButtonDown(Buttons.DPadUp);
		}
		return true;
	}

	public bool PreviousButtonStatus()
	{
		if (!previousState.IsButtonDown(Buttons.A) && !previousState.IsButtonDown(Buttons.B) && !previousState.IsButtonDown(Buttons.X) && !previousState.IsButtonDown(Buttons.Y) && !previousState.IsButtonDown(Buttons.Start) && !previousState.IsButtonDown(Buttons.Back) && !previousState.IsButtonDown(Buttons.LeftStick) && !previousState.IsButtonDown(Buttons.RightStick) && !previousState.IsButtonDown(Buttons.LeftShoulder) && !previousState.IsButtonDown(Buttons.RightShoulder) && !previousState.IsButtonDown(Buttons.DPadDown) && !previousState.IsButtonDown(Buttons.DPadLeft) && !previousState.IsButtonDown(Buttons.DPadRight))
		{
			return previousState.IsButtonDown(Buttons.DPadUp);
		}
		return true;
	}

	public bool ButtonPressed()
	{
		if (ButtonStatus())
		{
			return !PreviousButtonStatus();
		}
		return false;
	}

	public bool ButtonReleased()
	{
		if (!ButtonStatus())
		{
			return PreviousButtonStatus();
		}
		return false;
	}

	public static bool AnyButtonPressed()
	{
		if (!Instance(PlayerIndex.One).ButtonPressed() && !Instance(PlayerIndex.Two).ButtonPressed() && !Instance(PlayerIndex.Three).ButtonPressed())
		{
			return Instance(PlayerIndex.Four).ButtonPressed();
		}
		return true;
	}

	public static bool ButtonPressed(List<PlayerIndex> indices)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).ButtonPressed())
			{
				return true;
			}
		}
		return false;
	}

	public static bool AnyButtonReleased()
	{
		if (!Instance(PlayerIndex.One).ButtonReleased() && !Instance(PlayerIndex.Two).ButtonReleased() && !Instance(PlayerIndex.Three).ButtonReleased())
		{
			return Instance(PlayerIndex.Four).ButtonReleased();
		}
		return true;
	}

	public static bool ButtonReleased(List<PlayerIndex> indices)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).ButtonReleased())
			{
				return true;
			}
		}
		return false;
	}

	public static bool AnyButtonPressed(Buttons button)
	{
		if (!Instance(PlayerIndex.One).ButtonPressed(button) && !Instance(PlayerIndex.Two).ButtonPressed(button) && !Instance(PlayerIndex.Three).ButtonPressed(button))
		{
			return Instance(PlayerIndex.Four).ButtonPressed(button);
		}
		return true;
	}

	public static bool ButtonPressed(List<PlayerIndex> indices, Buttons button)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).ButtonPressed(button))
			{
				return true;
			}
		}
		return false;
	}

	public static bool AnyButtonPressed(Buttons button, ref PlayerIndex whoPressed)
	{
		for (int i = 0; i < 4; i++)
		{
			if (Instance((PlayerIndex)i).ButtonPressed(button))
			{
				whoPressed = (PlayerIndex)i;
				return true;
			}
		}
		return false;
	}

	public static bool ButtonPressed(List<PlayerIndex> indices, Buttons button, ref PlayerIndex whoPressed)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).ButtonPressed(button))
			{
				whoPressed = index;
				return true;
			}
		}
		return false;
	}

	public bool ButtonReleased(Buttons button)
	{
		if (!ButtonStatus(button))
		{
			return PreviousButtonStatus(button);
		}
		return false;
	}

	public static bool AnyButtonReleased(Buttons button)
	{
		if (!Instance(PlayerIndex.One).ButtonReleased(button) && !Instance(PlayerIndex.Two).ButtonReleased(button) && !Instance(PlayerIndex.Three).ButtonReleased(button))
		{
			return Instance(PlayerIndex.Four).ButtonReleased(button);
		}
		return true;
	}

	public static bool ButtonReleased(List<PlayerIndex> indices, Buttons button)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).ButtonReleased(button))
			{
				return true;
			}
		}
		return false;
	}

	public static bool AnyButtonReleased(Buttons button, ref PlayerIndex whoPressed)
	{
		for (int i = 0; i < 4; i++)
		{
			if (Instance((PlayerIndex)i).ButtonReleased(button))
			{
				whoPressed = (PlayerIndex)i;
				return true;
			}
		}
		return false;
	}

	public static bool ButtonReleased(List<PlayerIndex> indices, Buttons button, ref PlayerIndex whoPressed)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).ButtonReleased(button))
			{
				whoPressed = index;
				return true;
			}
		}
		return false;
	}

	public bool PreviousStickStatus(Stick axis, StickDirection direction)
	{
		Vector2 vector = PreviousStickPosition(axis);
		return direction switch
		{
			StickDirection.Up => vector.Y >= 0.5f, 
			StickDirection.Down => vector.Y <= -0.5f, 
			StickDirection.Left => vector.X <= -0.5f, 
			StickDirection.Right => vector.X >= 0.5f, 
			_ => false, 
		};
	}

	public bool StickStatus(Stick axis, StickDirection direction)
	{
		Vector2 vector = StickPosition(axis);
		return direction switch
		{
			StickDirection.Up => vector.Y >= 0.5f, 
			StickDirection.Down => vector.Y <= -0.5f, 
			StickDirection.Left => vector.X <= -0.5f, 
			StickDirection.Right => vector.X >= 0.5f, 
			_ => false, 
		};
	}

	public static bool AnyStickStatus(Stick axis, StickDirection direction)
	{
		if (!Instance(PlayerIndex.One).StickStatus(axis, direction) && !Instance(PlayerIndex.Two).StickStatus(axis, direction) && !Instance(PlayerIndex.Three).StickStatus(axis, direction))
		{
			return Instance(PlayerIndex.Four).StickStatus(axis, direction);
		}
		return true;
	}

	public static bool AnyStickStatus(Stick axis, StickDirection direction, ref PlayerIndex whoPressed)
	{
		for (int i = 0; i < 4; i++)
		{
			if (Instance((PlayerIndex)i).StickStatus(axis, direction))
			{
				whoPressed = (PlayerIndex)i;
				return true;
			}
		}
		return false;
	}

	public static bool StickStatus(List<PlayerIndex> indices, Stick axis, StickDirection direction)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).StickStatus(axis, direction))
			{
				return true;
			}
		}
		return false;
	}

	public static bool StickStatus(List<PlayerIndex> indices, Stick axis, StickDirection direction, ref PlayerIndex whoPressed)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).StickStatus(axis, direction))
			{
				whoPressed = index;
				return true;
			}
		}
		return false;
	}

	public bool StickPressed(Stick axis, StickDirection direction)
	{
		if (StickStatus(axis, direction))
		{
			return !PreviousStickStatus(axis, direction);
		}
		return false;
	}

	public static bool AnyStickPressed(Stick axis, StickDirection direction)
	{
		if (!Instance(PlayerIndex.One).StickPressed(axis, direction) && !Instance(PlayerIndex.Two).StickPressed(axis, direction) && !Instance(PlayerIndex.Three).StickPressed(axis, direction))
		{
			return Instance(PlayerIndex.Four).StickPressed(axis, direction);
		}
		return true;
	}

	public static bool AnyStickPressed(Stick axis, StickDirection direction, ref PlayerIndex whoPressed)
	{
		for (int i = 0; i < 4; i++)
		{
			if (Instance((PlayerIndex)i).StickPressed(axis, direction))
			{
				whoPressed = (PlayerIndex)i;
				return true;
			}
		}
		return false;
	}

	public static bool StickPressed(List<PlayerIndex> indices, Stick axis, StickDirection direction)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).StickPressed(axis, direction))
			{
				return true;
			}
		}
		return false;
	}

	public static bool StickPressed(List<PlayerIndex> indices, Stick axis, StickDirection direction, ref PlayerIndex whoPressed)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).StickPressed(axis, direction))
			{
				whoPressed = index;
				return true;
			}
		}
		return false;
	}

	public bool StickReleased(Stick axis, StickDirection direction)
	{
		if (!StickStatus(axis, direction))
		{
			return PreviousStickStatus(axis, direction);
		}
		return false;
	}

	public static bool AnyStickReleased(Stick axis, StickDirection direction)
	{
		if (!Instance(PlayerIndex.One).StickReleased(axis, direction) && !Instance(PlayerIndex.Two).StickReleased(axis, direction) && !Instance(PlayerIndex.Three).StickReleased(axis, direction))
		{
			return Instance(PlayerIndex.Four).StickReleased(axis, direction);
		}
		return true;
	}

	public static bool AnyStickReleased(Stick axis, StickDirection direction, ref PlayerIndex whoPressed)
	{
		for (int i = 0; i < 4; i++)
		{
			if (Instance((PlayerIndex)i).StickReleased(axis, direction))
			{
				whoPressed = (PlayerIndex)i;
				return true;
			}
		}
		return false;
	}

	public static bool StickReleased(List<PlayerIndex> indices, Stick axis, StickDirection direction)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).StickReleased(axis, direction))
			{
				return true;
			}
		}
		return false;
	}

	public Vector2 PreviousStickPosition(Stick axis)
	{
		Vector2 value = axis switch
		{
			Stick.RightStick => previousState.ThumbSticks.Right, 
			_ => previousState.ThumbSticks.Left, 
		};
		DeadZone.GetStickValue(ref value, out value);
		return value;
	}

	public Vector2 StickPosition(Stick axis)
	{
		Vector2 value = axis switch
		{
			Stick.RightStick => currentState.ThumbSticks.Right, 
			_ => currentState.ThumbSticks.Left, 
		};
		DeadZone.GetStickValue(ref value, out value);
		return value;
	}

	public static Vector2 SumStickPositions(Stick axis)
	{
		return Instance(PlayerIndex.One).StickPosition(axis) + Instance(PlayerIndex.Two).StickPosition(axis) + Instance(PlayerIndex.Three).StickPosition(axis) + Instance(PlayerIndex.Four).StickPosition(axis);
	}

	public static Vector2 SumStickPositions(Stick axis, ref PlayerIndex whoPressed)
	{
		Vector2 zero = Vector2.Zero;
		for (int i = 0; i < 4; i++)
		{
			Vector2 vector = Instance((PlayerIndex)i).StickPosition(axis);
			if (!GameMath.ZeroLength(vector))
			{
				whoPressed = (PlayerIndex)i;
			}
			zero += vector;
		}
		return zero;
	}

	public static Vector2 SumStickPositions(List<PlayerIndex> indices, Stick axis)
	{
		Vector2 zero = Vector2.Zero;
		foreach (PlayerIndex index in indices)
		{
			zero += Instance(index).StickPosition(axis);
		}
		return zero;
	}

	public static Vector2 SumStickPositions(List<PlayerIndex> indices, Stick axis, ref PlayerIndex whoPressed)
	{
		Vector2 zero = Vector2.Zero;
		foreach (PlayerIndex index in indices)
		{
			Vector2 vector = Instance(index).StickPosition(axis);
			if (!GameMath.ZeroLength(vector))
			{
				whoPressed = index;
			}
			zero += vector;
		}
		return zero;
	}

	public static float SumTriggerPositions(Trigger trigger, ref PlayerIndex whoPressed)
	{
		float num = 0f;
		for (int i = 0; i < 4; i++)
		{
			float num2 = Instance((PlayerIndex)i).TriggerPosition(trigger);
			if (num2 != 0f)
			{
				whoPressed = (PlayerIndex)i;
			}
			num += num2;
		}
		return num;
	}

	public static float SumTriggerPositions(List<PlayerIndex> indices, Trigger trigger, ref PlayerIndex whoPressed)
	{
		float num = 0f;
		foreach (PlayerIndex index in indices)
		{
			float num2 = Instance(index).TriggerPosition(trigger);
			if (num2 != 0f)
			{
				whoPressed = index;
			}
			num += num2;
		}
		return num;
	}

	public static float SumTriggerPositions(Trigger trigger)
	{
		return Instance(PlayerIndex.One).TriggerPosition(trigger) + Instance(PlayerIndex.Two).TriggerPosition(trigger) + Instance(PlayerIndex.Three).TriggerPosition(trigger) + Instance(PlayerIndex.Four).TriggerPosition(trigger);
	}

	public bool TriggerStatus(Trigger trigger)
	{
		return TriggerPosition(trigger) >= 0.5f;
	}

	public static bool AnyTriggerStatus(Trigger trigger)
	{
		if (!Instance(PlayerIndex.One).TriggerStatus(trigger) && !Instance(PlayerIndex.Two).TriggerStatus(trigger) && !Instance(PlayerIndex.Three).TriggerStatus(trigger))
		{
			return Instance(PlayerIndex.Four).TriggerStatus(trigger);
		}
		return true;
	}

	public static bool TriggerStatus(List<PlayerIndex> indices, Trigger trigger)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).TriggerStatus(trigger))
			{
				return true;
			}
		}
		return false;
	}

	public static bool AnyTriggerStatus(Trigger trigger, ref PlayerIndex whoPressed)
	{
		for (int i = 0; i < 4; i++)
		{
			if (Instance((PlayerIndex)i).TriggerStatus(trigger))
			{
				whoPressed = (PlayerIndex)i;
				return true;
			}
		}
		return false;
	}

	public static bool AnyTriggerPressed(Trigger trigger, ref PlayerIndex whoPressed)
	{
		for (int i = 0; i < 4; i++)
		{
			if (Instance((PlayerIndex)i).TriggerPressed(trigger))
			{
				whoPressed = (PlayerIndex)i;
				return true;
			}
		}
		return false;
	}

	public static bool AnyTriggerReleased(Trigger trigger, ref PlayerIndex whoPressed)
	{
		for (int i = 0; i < 4; i++)
		{
			if (Instance((PlayerIndex)i).TriggerReleased(trigger))
			{
				whoPressed = (PlayerIndex)i;
				return true;
			}
		}
		return false;
	}

	public static bool TriggerStatus(List<PlayerIndex> indices, Trigger trigger, ref PlayerIndex whoPressed)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).TriggerStatus(trigger))
			{
				whoPressed = index;
				return true;
			}
		}
		return false;
	}

	public bool PreviousTriggerStatus(Trigger trigger)
	{
		return PreviousTriggerPosition(trigger) >= 0.5f;
	}

	public bool TriggerPressed(Trigger trigger)
	{
		if (TriggerStatus(trigger))
		{
			return !PreviousTriggerStatus(trigger);
		}
		return false;
	}

	public bool TriggerReleased(Trigger trigger)
	{
		if (!TriggerStatus(trigger))
		{
			return PreviousTriggerStatus(trigger);
		}
		return false;
	}

	public static bool TriggerReleased(List<PlayerIndex> indices, Trigger trigger, ref PlayerIndex whoPressed)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).TriggerReleased(trigger))
			{
				whoPressed = index;
				return true;
			}
		}
		return false;
	}

	public static bool TriggerPressed(List<PlayerIndex> indices, Trigger trigger)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).TriggerPressed(trigger))
			{
				return true;
			}
		}
		return false;
	}

	public static bool TriggerPressed(List<PlayerIndex> indices, Trigger trigger, ref PlayerIndex whoPressed)
	{
		foreach (PlayerIndex index in indices)
		{
			if (Instance(index).TriggerPressed(trigger))
			{
				whoPressed = index;
				return true;
			}
		}
		return false;
	}

	public float TriggerPosition(Trigger trigger)
	{
		return trigger switch
		{
			Trigger.RightTrigger => DeadZone.GetTriggerValue(currentState.Triggers.Right), 
			_ => DeadZone.GetTriggerValue(currentState.Triggers.Left), 
		};
	}

	public static float TriggerPosition(List<PlayerIndex> indices, Trigger trigger)
	{
		float num = 0f;
		foreach (PlayerIndex index in indices)
		{
			num = Math.Max(Instance(index).TriggerPosition(trigger), num);
		}
		return num;
	}

	public float PreviousTriggerPosition(Trigger trigger)
	{
		return trigger switch
		{
			Trigger.RightTrigger => DeadZone.GetTriggerValue(previousState.Triggers.Right), 
			_ => DeadZone.GetTriggerValue(previousState.Triggers.Left), 
		};
	}

	public void setVibration(float vibration)
	{
		setVibration(vibration, vibration);
	}

	public void vibrateFor(float leftVibration, float rightVibration, uint milliseconds)
	{
		setVibration(leftVibration, rightVibration);
		vibrateSpeed = new Vector2(leftVibration, rightVibration);
		vibrateEnd = Timer.DefaultTimer.TotalTime + milliseconds;
	}

	private void CheckTimedVibration()
	{
		if (vibrateEnd == 0 || !Connected)
		{
			return;
		}
		if (Timer.DefaultTimer.TotalTime > vibrateEnd)
		{
			if (setGamepadVibration(0f, 0f))
			{
				vibrateEnd = 0L;
			}
		}
		else
		{
			setGamepadVibration(vibrateSpeed.X, vibrateSpeed.Y);
		}
	}

	public void setVibration(float leftVibration, float rightVibration)
	{
		if (Connected)
		{
			setGamepadVibration(leftVibration, rightVibration);
			vibrateEnd = 0L;
		}
	}

	protected bool setGamepadVibration(float leftVibration, float rightVibration)
	{
		if (!Connected)
		{
			return false;
		}
		if (VibrationEnabled)
		{
			return GamePad.SetVibration(index, leftVibration, rightVibration);
		}
		return false;
	}

	public void StopVibration()
	{
		if (Connected && !HasTimedVibration)
		{
			setVibration(0f);
		}
	}

	public static void StopVibrations()
	{
		for (int i = 0; i < 4; i++)
		{
			Instance((PlayerIndex)i).StopVibration();
		}
	}
}
