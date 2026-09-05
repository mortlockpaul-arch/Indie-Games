using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace _2d_house_of_terror;

public static class controllers
{
	public static bool[] connected;

	private static int[] buttons;

	private static int[] clicked_buttons;

	public static float[] rtrigger;

	public static float[] ltrigger;

	public static Vector2[] lthumb;

	public static Vector2[] rthumb;

	public static void update()
	{
		for (int i = 0; i < 4; i++)
		{
			int num = buttons[i];
			PlayerIndex playerIndex = i switch
			{
				2 => PlayerIndex.Three, 
				1 => PlayerIndex.Two, 
				0 => PlayerIndex.One, 
				_ => PlayerIndex.Four, 
			};
			GamePadState state = GamePad.GetState(playerIndex);
			GamePadThumbSticks thumbSticks = GamePad.GetState(playerIndex).ThumbSticks;
			buttons[i] = 0;
			if (state.Buttons.X == ButtonState.Pressed)
			{
				buttons[i] |= 4;
			}
			if (state.Buttons.Y == ButtonState.Pressed)
			{
				buttons[i] |= 8;
			}
			if (state.Buttons.A == ButtonState.Pressed)
			{
				buttons[i] |= 1;
			}
			if (state.Buttons.B == ButtonState.Pressed)
			{
				buttons[i] |= 2;
			}
			if (state.Buttons.LeftShoulder == ButtonState.Pressed)
			{
				buttons[i] |= 256;
			}
			if (state.Buttons.RightShoulder == ButtonState.Pressed)
			{
				buttons[i] |= 512;
			}
			if (state.Buttons.LeftStick == ButtonState.Pressed)
			{
				buttons[i] |= 1024;
			}
			if (state.Buttons.RightStick == ButtonState.Pressed)
			{
				buttons[i] |= 2048;
			}
			if (state.DPad.Down == ButtonState.Pressed)
			{
				buttons[i] |= 16;
			}
			if (state.DPad.Up == ButtonState.Pressed)
			{
				buttons[i] |= 32;
			}
			if (state.DPad.Left == ButtonState.Pressed)
			{
				buttons[i] |= 64;
			}
			if (state.DPad.Right == ButtonState.Pressed)
			{
				buttons[i] |= 128;
			}
			if (state.Buttons.Start == ButtonState.Pressed)
			{
				buttons[i] |= 4096;
			}
			if (state.Buttons.Back == ButtonState.Pressed)
			{
				buttons[i] |= 8192;
			}
			if ((double)thumbSticks.Left.Y > 0.5)
			{
				buttons[i] |= 16384;
			}
			if ((double)thumbSticks.Left.Y < -0.5)
			{
				buttons[i] |= 32768;
			}
			if ((double)thumbSticks.Left.X > 0.5)
			{
				buttons[i] |= 131072;
			}
			if ((double)thumbSticks.Left.X < -0.5)
			{
				buttons[i] |= 65536;
			}
			if ((double)thumbSticks.Right.Y > 0.5)
			{
				buttons[i] |= 262144;
			}
			if ((double)thumbSticks.Right.Y < -0.5)
			{
				buttons[i] |= 524288;
			}
			if ((double)thumbSticks.Right.X > 0.5)
			{
				buttons[i] |= 2097152;
			}
			if ((double)thumbSticks.Right.X < -0.5)
			{
				buttons[i] |= 1048576;
			}
			ref Vector2 reference = ref rthumb[i];
			reference = thumbSticks.Right;
			ref Vector2 reference2 = ref lthumb[i];
			reference2 = thumbSticks.Left;
			rtrigger[i] = state.Triggers.Right;
			ltrigger[i] = state.Triggers.Left;
			clicked_buttons[i] = ~num & (num ^ buttons[i]);
			connected[i] = state.IsConnected;
		}
	}

	public static bool clicked(int con_id, CONTROLLER_BUTTONS btn)
	{
		if (con_id < 0 || con_id > 3)
		{
			return false;
		}
		return ((uint)clicked_buttons[con_id] & (uint)btn) != 0;
	}

	public static bool clicked(CONTROLLER_BUTTONS btn)
	{
		for (int i = 0; i < 4; i++)
		{
			if (((uint)clicked_buttons[i] & (uint)btn) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public static bool pressed(int con_id, CONTROLLER_BUTTONS btn)
	{
		if (con_id < 0 || con_id > 3)
		{
			return false;
		}
		return ((uint)buttons[con_id] & (uint)btn) != 0;
	}

	public static bool pressed(CONTROLLER_BUTTONS btn)
	{
		for (int i = 0; i < 4; i++)
		{
			if (((uint)buttons[i] & (uint)btn) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public static void reset()
	{
		for (int i = 0; i < 4; i++)
		{
			buttons[i] = 0;
			clicked_buttons[i] = 0;
		}
	}

	static controllers()
	{
		bool[] array = new bool[4];
		connected = array;
		int[] array2 = new int[4];
		buttons = array2;
		int[] array3 = new int[4];
		clicked_buttons = array3;
		float[] array4 = new float[4];
		rtrigger = array4;
		float[] array5 = new float[4];
		ltrigger = array5;
		lthumb = new Vector2[4]
		{
			new Vector2(0f),
			new Vector2(0f),
			new Vector2(0f),
			new Vector2(0f)
		};
		rthumb = new Vector2[4]
		{
			new Vector2(0f),
			new Vector2(0f),
			new Vector2(0f),
			new Vector2(0f)
		};
	}
}
