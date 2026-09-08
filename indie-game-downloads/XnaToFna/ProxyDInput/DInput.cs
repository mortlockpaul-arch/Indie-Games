using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XnaToFna.ProxyDInput;

public static class DInput
{
	public static bool IsProxy = Environment.GetEnvironmentVariable("XTF_PROXY_DINPUT") == "1";

	public static DInputState[] States = new DInputState[0];

	public static DInputState StateDefault = new DInputState();

	public static bool Initialize()
	{
		if (!IsProxy)
		{
			XnaToFnaHelper.Log("[ProxyDInput] ProxyDInput disabled by default - 'export XTF_PROXY_DINPUT=1' to enable");
			return false;
		}
		XnaToFnaHelper.Log("[ProxyDInput] Initializing ProxyDInput");
		States = new DInputState[XnaToFnaHelper.MaximumGamepadCount];
		for (int i = 0; i < States.Length; i++)
		{
			States[i] = new DInputState();
		}
		return true;
	}

	public static void Terminate()
	{
	}

	public static void EnumGamepads()
	{
	}

	public static void Update()
	{
		for (int i = 0; i < States.Length; i++)
		{
			GamePadState state = GamePad.GetState((PlayerIndex)i);
			DInputState dInputState = States[i];
			dInputState.connected = state.IsConnected;
			if (dInputState.connected)
			{
				GamePadThumbSticks thumbSticks = state.ThumbSticks;
				dInputState.leftX = thumbSticks.Left.X;
				dInputState.leftY = thumbSticks.Left.Y;
				dInputState.leftZ = 0f;
				dInputState.rightX = thumbSticks.Right.X;
				dInputState.rightY = thumbSticks.Right.Y;
				dInputState.rightZ = 0f;
				GamePadTriggers triggers = state.Triggers;
				dInputState.slider1 = state.Triggers.Left;
				dInputState.slider2 = state.Triggers.Right;
				GamePadDPad dPad = state.DPad;
				dInputState.left = dPad.Left == ButtonState.Pressed;
				dInputState.right = dPad.Right == ButtonState.Pressed;
				dInputState.up = dPad.Up == ButtonState.Pressed;
				dInputState.down = dPad.Down == ButtonState.Pressed;
				GamePadButtons buttons = state.Buttons;
				List<bool> list = dInputState.buttons ?? new List<bool>();
				for (int j = list.Count; j < 13; j++)
				{
					list.Add(item: false);
				}
				while (list.Count > 13)
				{
					list.RemoveAt(0);
				}
				list[0] = buttons.X == ButtonState.Pressed;
				list[1] = buttons.A == ButtonState.Pressed;
				list[2] = buttons.B == ButtonState.Pressed;
				list[3] = buttons.Y == ButtonState.Pressed;
				list[4] = buttons.LeftShoulder == ButtonState.Pressed;
				list[5] = buttons.RightShoulder == ButtonState.Pressed;
				list[6] = triggers.Left >= 0.999f;
				list[7] = triggers.Right >= 0.999f;
				list[8] = buttons.Back == ButtonState.Pressed;
				list[9] = buttons.Start == ButtonState.Pressed;
				list[10] = buttons.BigButton == ButtonState.Pressed;
				list[11] = buttons.LeftStick == ButtonState.Pressed;
				list[12] = buttons.RightStick == ButtonState.Pressed;
				dInputState.buttons = list;
			}
		}
	}

	public static DInputState GetState(int player)
	{
		if (player < States.Length)
		{
			return States[player];
		}
		return StateDefault;
	}

	public static string GetProductName(int player)
	{
		if (player >= States.Length)
		{
			return string.Empty;
		}
		return $"ProxyDInput #{player + 1}";
	}

	public static string GetProductGUID(int player)
	{
		if (player < States.Length)
		{
			return GamePad.GetGUIDEXT((PlayerIndex)player);
		}
		return string.Empty;
	}
}
