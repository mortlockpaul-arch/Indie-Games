using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

public static class InputManager
{
	public const int POWERUP_USE_KEY = 6;

	public const int RUN_KEY = 10;

	public const int JUMP_KEY = 4;

	public const int SHOOT_FORWARD_KEY = 5;

	public const int SHOOT_UP_KEY = 7;

	public const int SHOOT_HIGH_KEY = 6;

	public const int DISPLAY_NAME_KEY = 12;

	public const int MENU_VALID_KEY = 8;

	public const int MENU_SELECT_KEY = 4;

	public const int MENU_BACK_KEY = 5;

	public const int PAUSE_KEY = 8;

	public const int TOGGLE_COLLISION_KEY = 13;

	public const int MAX_PLAYERS = 4;

	public const int MAX_CONTROLLER = 8;

	public static float INPUT_LATENCY = 200f;

	public static int[] GamerIndex = new int[4] { -1, -1, -1, -1 };

	public static bool m_bAllowControl = true;

	public static ButtonState[][] Controller = new ButtonState[8][]
	{
		new ButtonState[14],
		new ButtonState[14],
		new ButtonState[14],
		new ButtonState[14],
		new ButtonState[14],
		new ButtonState[14],
		new ButtonState[14],
		new ButtonState[14]
	};

	private static bool[] PadLock = new bool[8];

	public static ButtonState GetKeyState(PlayerIndex index, int key)
	{
		return Controller[(int)index][key];
	}

	public static void SetLockPad(int index, bool block)
	{
		PadLock[index] = block;
		for (int i = 0; i < Controller[index].Length; i++)
		{
			Controller[index][i] = ButtonState.Released;
		}
	}

	public static void SetKeyState(PlayerIndex index, int key, bool pressed)
	{
		if (pressed)
		{
			Controller[(int)index][key] = ButtonState.Pressed;
		}
		else
		{
			Controller[(int)index][key] = ButtonState.Released;
		}
	}

	public static void Update(int pIndex)
	{
		if (GamePad.GetState((PlayerIndex)pIndex).IsConnected && !PadLock[pIndex])
		{
			Controller[pIndex][0] = GamePad.GetState((PlayerIndex)pIndex).DPad.Up;
			Controller[pIndex][1] = GamePad.GetState((PlayerIndex)pIndex).DPad.Left;
			Controller[pIndex][2] = GamePad.GetState((PlayerIndex)pIndex).DPad.Down;
			Controller[pIndex][3] = GamePad.GetState((PlayerIndex)pIndex).DPad.Right;
			Controller[pIndex][4] = GamePad.GetState((PlayerIndex)pIndex).Buttons.A;
			Controller[pIndex][5] = GamePad.GetState((PlayerIndex)pIndex).Buttons.B;
			Controller[pIndex][6] = GamePad.GetState((PlayerIndex)pIndex).Buttons.X;
			Controller[pIndex][7] = GamePad.GetState((PlayerIndex)pIndex).Buttons.Y;
			Controller[pIndex][8] = GamePad.GetState((PlayerIndex)pIndex).Buttons.Start;
			Controller[pIndex][9] = GamePad.GetState((PlayerIndex)pIndex).Buttons.Back;
			Controller[pIndex][10] = (ButtonState)GamePad.GetState((PlayerIndex)pIndex).Triggers.Right;
			Controller[pIndex][11] = (ButtonState)GamePad.GetState((PlayerIndex)pIndex).Triggers.Left;
			Controller[pIndex][12] = GamePad.GetState((PlayerIndex)pIndex).Buttons.RightShoulder;
			Controller[pIndex][13] = GamePad.GetState((PlayerIndex)pIndex).Buttons.LeftShoulder;
			if (GamePad.GetState((PlayerIndex)pIndex).ThumbSticks.Left.Y < 0f)
			{
				Controller[pIndex][2] = ButtonState.Pressed;
			}
			if (GamePad.GetState((PlayerIndex)pIndex).ThumbSticks.Left.Y > 0f)
			{
				Controller[pIndex][0] = ButtonState.Pressed;
			}
			if (GamePad.GetState((PlayerIndex)pIndex).ThumbSticks.Left.X > 0f)
			{
				Controller[pIndex][3] = ButtonState.Pressed;
			}
			if (GamePad.GetState((PlayerIndex)pIndex).ThumbSticks.Left.X < 0f)
			{
				Controller[pIndex][1] = ButtonState.Pressed;
			}
		}
	}
}
