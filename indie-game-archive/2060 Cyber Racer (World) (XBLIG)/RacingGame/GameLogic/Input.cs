using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using RacingGame.Graphics;

namespace RacingGame.GameLogic;

public static class Input
{
	private static bool mouseDetected;

	private static KeyboardState keyboardState;

	private static List<Keys> keysPressedLastFrame;

	private static GamePadState gamePadState;

	private static GamePadState gamePadStateLastFrame;

	private static int mouseWheelDelta;

	private static Point startDraggingPos;

	public static PlayerIndex controllingPlayer;

	public static bool MouseDetected => mouseDetected;

	public static Point MousePos
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Point.Zero;
		}
	}

	public static float MouseXMovement => 0f;

	public static float MouseYMovement => 0f;

	public static bool HasMouseMoved => false;

	public static bool MouseLeftButtonPressed => false;

	public static bool MouseRightButtonPressed => false;

	public static bool MouseMiddleButtonPressed => false;

	public static bool MouseLeftButtonJustPressed => false;

	public static bool MouseRightButtonJustPressed => false;

	public static Point MouseDraggingAmount
	{
		get
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			return new Point(startDraggingPos.X - MousePos.X, startDraggingPos.Y - MousePos.Y);
		}
	}

	public static int MouseWheelDelta => mouseWheelDelta;

	public static KeyboardState Keyboard
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return keyboardState;
		}
	}

	public static bool KeyboardSpaceJustPressed
	{
		get
		{
			if (((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)32))
			{
				return !keysPressedLastFrame.Contains((Keys)32);
			}
			return false;
		}
	}

	public static bool KeyboardF1JustPressed
	{
		get
		{
			if (((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)112))
			{
				return !keysPressedLastFrame.Contains((Keys)112);
			}
			return false;
		}
	}

	public static bool KeyboardEscapeJustPressed
	{
		get
		{
			if (((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)27))
			{
				return !keysPressedLastFrame.Contains((Keys)27);
			}
			return false;
		}
	}

	public static bool KeyboardLeftJustPressed
	{
		get
		{
			if (((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)37))
			{
				return !keysPressedLastFrame.Contains((Keys)37);
			}
			return false;
		}
	}

	public static bool KeyboardRightJustPressed
	{
		get
		{
			if (((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)39))
			{
				return !keysPressedLastFrame.Contains((Keys)39);
			}
			return false;
		}
	}

	public static bool KeyboardUpJustPressed
	{
		get
		{
			if (((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)38))
			{
				return !keysPressedLastFrame.Contains((Keys)38);
			}
			return false;
		}
	}

	public static bool KeyboardDownJustPressed
	{
		get
		{
			if (((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)40))
			{
				return !keysPressedLastFrame.Contains((Keys)40);
			}
			return false;
		}
	}

	public static bool KeyboardLeftPressed => ((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)37);

	public static bool KeyboardRightPressed => ((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)39);

	public static bool KeyboardUpPressed => ((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)38);

	public static bool KeyboardDownPressed => ((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)40);

	public static GamePadState GamePad
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return gamePadState;
		}
	}

	public static bool IsGamePadConnected => ((GamePadState)(ref gamePadState)).IsConnected;

	public static bool GamePadStartPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			GamePadButtons buttons = ((GamePadState)(ref gamePadState)).Buttons;
			return (int)((GamePadButtons)(ref buttons)).Start == 1;
		}
	}

	public static bool GamePadStartJustPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Invalid comparison between Unknown and I4
			GamePadButtons buttons = ((GamePadState)(ref gamePadState)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 1)
			{
				GamePadButtons buttons2 = ((GamePadState)(ref gamePadStateLastFrame)).Buttons;
				return (int)((GamePadButtons)(ref buttons2)).Start == 0;
			}
			return false;
		}
	}

	public static bool GamePadAPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			GamePadButtons buttons = ((GamePadState)(ref gamePadState)).Buttons;
			return (int)((GamePadButtons)(ref buttons)).A == 1;
		}
	}

	public static bool GamePadBPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			GamePadButtons buttons = ((GamePadState)(ref gamePadState)).Buttons;
			return (int)((GamePadButtons)(ref buttons)).B == 1;
		}
	}

	public static bool GamePadXPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			GamePadButtons buttons = ((GamePadState)(ref gamePadState)).Buttons;
			return (int)((GamePadButtons)(ref buttons)).X == 1;
		}
	}

	public static bool GamePadYPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			GamePadButtons buttons = ((GamePadState)(ref gamePadState)).Buttons;
			return (int)((GamePadButtons)(ref buttons)).Y == 1;
		}
	}

	public static bool GamePadLeftPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			GamePadDPad dPad = ((GamePadState)(ref gamePadState)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Left != 1)
			{
				GamePadThumbSticks thumbSticks = ((GamePadState)(ref gamePadState)).ThumbSticks;
				return ((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.75f;
			}
			return true;
		}
	}

	public static bool GamePadRightPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			GamePadDPad dPad = ((GamePadState)(ref gamePadState)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Right != 1)
			{
				GamePadThumbSticks thumbSticks = ((GamePadState)(ref gamePadState)).ThumbSticks;
				return ((GamePadThumbSticks)(ref thumbSticks)).Left.X > 0.75f;
			}
			return true;
		}
	}

	public static bool GamePadLeftJustPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			GamePadDPad dPad = ((GamePadState)(ref gamePadState)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Left == 1)
			{
				GamePadDPad dPad2 = ((GamePadState)(ref gamePadStateLastFrame)).DPad;
				if ((int)((GamePadDPad)(ref dPad2)).Left == 0)
				{
					return true;
				}
			}
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref gamePadState)).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.75f)
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref gamePadStateLastFrame)).ThumbSticks;
				return ((GamePadThumbSticks)(ref thumbSticks2)).Left.X > -0.75f;
			}
			return false;
		}
	}

	public static bool GamePadRightJustPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			GamePadDPad dPad = ((GamePadState)(ref gamePadState)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Right == 1)
			{
				GamePadDPad dPad2 = ((GamePadState)(ref gamePadStateLastFrame)).DPad;
				if ((int)((GamePadDPad)(ref dPad2)).Right == 0)
				{
					return true;
				}
			}
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref gamePadState)).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks)).Left.X > 0.75f)
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref gamePadStateLastFrame)).ThumbSticks;
				return ((GamePadThumbSticks)(ref thumbSticks2)).Left.X < 0.75f;
			}
			return false;
		}
	}

	public static bool GamePadUpJustPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			GamePadDPad dPad = ((GamePadState)(ref gamePadState)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Up == 1)
			{
				GamePadDPad dPad2 = ((GamePadState)(ref gamePadStateLastFrame)).DPad;
				if ((int)((GamePadDPad)(ref dPad2)).Up == 0)
				{
					return true;
				}
			}
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref gamePadState)).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks)).Left.Y > 0.75f)
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref gamePadStateLastFrame)).ThumbSticks;
				return ((GamePadThumbSticks)(ref thumbSticks2)).Left.Y < 0.75f;
			}
			return false;
		}
	}

	public static bool GamePadDownJustPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			GamePadDPad dPad = ((GamePadState)(ref gamePadState)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Down == 1)
			{
				GamePadDPad dPad2 = ((GamePadState)(ref gamePadStateLastFrame)).DPad;
				if ((int)((GamePadDPad)(ref dPad2)).Down == 0)
				{
					return true;
				}
			}
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref gamePadState)).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.75f)
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref gamePadStateLastFrame)).ThumbSticks;
				return ((GamePadThumbSticks)(ref thumbSticks2)).Left.Y > -0.75f;
			}
			return false;
		}
	}

	public static bool GamePadUpPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			GamePadDPad dPad = ((GamePadState)(ref gamePadState)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Up != 1)
			{
				GamePadThumbSticks thumbSticks = ((GamePadState)(ref gamePadState)).ThumbSticks;
				return ((GamePadThumbSticks)(ref thumbSticks)).Left.Y > 0.75f;
			}
			return true;
		}
	}

	public static bool GamePadDownPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			GamePadDPad dPad = ((GamePadState)(ref gamePadState)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Down != 1)
			{
				GamePadThumbSticks thumbSticks = ((GamePadState)(ref gamePadState)).ThumbSticks;
				return ((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.75f;
			}
			return true;
		}
	}

	public static bool GamePadAJustPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Invalid comparison between Unknown and I4
			GamePadButtons buttons = ((GamePadState)(ref gamePadState)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).A == 1)
			{
				GamePadButtons buttons2 = ((GamePadState)(ref gamePadStateLastFrame)).Buttons;
				return (int)((GamePadButtons)(ref buttons2)).A == 0;
			}
			return false;
		}
	}

	public static bool GamePadBJustPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Invalid comparison between Unknown and I4
			GamePadButtons buttons = ((GamePadState)(ref gamePadState)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).B == 1)
			{
				GamePadButtons buttons2 = ((GamePadState)(ref gamePadStateLastFrame)).Buttons;
				return (int)((GamePadButtons)(ref buttons2)).B == 0;
			}
			return false;
		}
	}

	public static bool GamePadXJustPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Invalid comparison between Unknown and I4
			GamePadButtons buttons = ((GamePadState)(ref gamePadState)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).X == 1)
			{
				GamePadButtons buttons2 = ((GamePadState)(ref gamePadStateLastFrame)).Buttons;
				return (int)((GamePadButtons)(ref buttons2)).X == 0;
			}
			return false;
		}
	}

	public static bool GamePadYJustPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Invalid comparison between Unknown and I4
			GamePadButtons buttons = ((GamePadState)(ref gamePadState)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Y == 1)
			{
				GamePadButtons buttons2 = ((GamePadState)(ref gamePadStateLastFrame)).Buttons;
				return (int)((GamePadButtons)(ref buttons2)).Y == 0;
			}
			return false;
		}
	}

	public static bool GamePadBackJustPressed
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Invalid comparison between Unknown and I4
			GamePadButtons buttons = ((GamePadState)(ref gamePadState)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Back == 1)
			{
				GamePadButtons buttons2 = ((GamePadState)(ref gamePadStateLastFrame)).Buttons;
				return (int)((GamePadButtons)(ref buttons2)).Back == 0;
			}
			return false;
		}
	}

	public static void ResetMouseDraggingAmount()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		startDraggingPos = MousePos;
	}

	public static bool MouseInBox(Rectangle rect)
	{
		return false;
	}

	public static bool MouseInBoxRelative(Rectangle rect)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)BaseGame.Width / 1024f;
		float num2 = (float)BaseGame.Height / 640f;
		return MouseInBox(new Rectangle((int)Math.Round((float)rect.X * num), (int)Math.Round((float)rect.Y * num2), (int)Math.Round((float)((Rectangle)(ref rect)).Right * num), (int)Math.Round((float)((Rectangle)(ref rect)).Bottom * num2)));
	}

	public static bool IsSpecialKey(Keys key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Expected I4, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Invalid comparison between Unknown and I4
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Invalid comparison between Unknown and I4
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		int num = (int)key;
		if ((num >= 65 && num <= 90) || (num >= 48 && num <= 57) || (int)key == 32 || (int)key == 192 || (int)key == 189 || (int)key == 220 || (int)key == 219 || (int)key == 221 || (int)key == 222 || (int)key == 191 || (int)key == 187)
		{
			return false;
		}
		return true;
	}

	public static char KeyToChar(Keys key, bool shiftPressed)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Expected I4, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Invalid comparison between Unknown and I4
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Invalid comparison between Unknown and I4
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Invalid comparison between Unknown and I4
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Invalid comparison between Unknown and I4
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Invalid comparison between Unknown and I4
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Invalid comparison between Unknown and I4
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Invalid comparison between Unknown and I4
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Invalid comparison between Unknown and I4
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Invalid comparison between Unknown and I4
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Invalid comparison between Unknown and I4
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Invalid comparison between Unknown and I4
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Invalid comparison between Unknown and I4
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Invalid comparison between Unknown and I4
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Invalid comparison between Unknown and I4
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Invalid comparison between Unknown and I4
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Invalid comparison between Unknown and I4
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Invalid comparison between Unknown and I4
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Invalid comparison between Unknown and I4
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Invalid comparison between Unknown and I4
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Invalid comparison between Unknown and I4
		char result = ' ';
		int num = (int)key;
		if (num >= 65 && num <= 90)
		{
			result = ((!shiftPressed) ? ((object)key).ToString().ToLower()[0] : ((object)key).ToString()[0]);
		}
		else if (num >= 48 && num <= 57 && !shiftPressed)
		{
			result = (char)(48 + (num - 48));
		}
		else if ((int)key == 49 && shiftPressed)
		{
			result = '!';
		}
		else if ((int)key == 50 && shiftPressed)
		{
			result = '@';
		}
		else if ((int)key == 51 && shiftPressed)
		{
			result = '#';
		}
		else if ((int)key == 52 && shiftPressed)
		{
			result = '$';
		}
		else if ((int)key == 53 && shiftPressed)
		{
			result = '%';
		}
		else if ((int)key == 54 && shiftPressed)
		{
			result = '^';
		}
		else if ((int)key == 55 && shiftPressed)
		{
			result = '&';
		}
		else if ((int)key == 56 && shiftPressed)
		{
			result = '*';
		}
		else if ((int)key == 57 && shiftPressed)
		{
			result = '(';
		}
		else if ((int)key == 48 && shiftPressed)
		{
			result = ')';
		}
		else if ((int)key == 192)
		{
			result = (shiftPressed ? '~' : '`');
		}
		else if ((int)key == 189)
		{
			result = (shiftPressed ? '_' : '-');
		}
		else if ((int)key == 220)
		{
			result = (shiftPressed ? '|' : '\\');
		}
		else if ((int)key == 219)
		{
			result = (shiftPressed ? '{' : '[');
		}
		else if ((int)key == 221)
		{
			result = (shiftPressed ? '}' : ']');
		}
		else if ((int)key == 186)
		{
			result = (shiftPressed ? ':' : ';');
		}
		else if ((int)key == 222)
		{
			result = (shiftPressed ? '"' : '\'');
		}
		else if ((int)key == 188)
		{
			result = (shiftPressed ? '<' : '.');
		}
		else if ((int)key == 190)
		{
			result = (shiftPressed ? '>' : ',');
		}
		else if ((int)key == 191)
		{
			result = (shiftPressed ? '?' : '/');
		}
		else if ((int)key == 187)
		{
			result = (shiftPressed ? '+' : '=');
		}
		return result;
	}

	public static void HandleKeyboardInput(ref string inputText)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Invalid comparison between Unknown and I4
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		bool shiftPressed = ((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)160) || ((KeyboardState)(ref keyboardState)).IsKeyDown((Keys)161);
		Keys[] pressedKeys = ((KeyboardState)(ref keyboardState)).GetPressedKeys();
		foreach (Keys val in pressedKeys)
		{
			if (!keysPressedLastFrame.Contains(val))
			{
				if (!IsSpecialKey(val) && inputText.Length < 32)
				{
					inputText += KeyToChar(val, shiftPressed);
				}
				else if ((int)val == 8 && inputText.Length > 0)
				{
					inputText = inputText.Substring(0, inputText.Length - 1);
				}
			}
		}
	}

	public static bool KeyboardKeyJustPressed(Keys key)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (((KeyboardState)(ref keyboardState)).IsKeyDown(key))
		{
			return !keysPressedLastFrame.Contains(key);
		}
		return false;
	}

	internal static void Update()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		mouseDetected = false;
		keysPressedLastFrame = new List<Keys>(((KeyboardState)(ref keyboardState)).GetPressedKeys());
		keyboardState = Keyboard.GetState();
		gamePadStateLastFrame = gamePadState;
		for (int i = 0; i < 4; i++)
		{
			if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers)[i] != null)
			{
				gamePadState = GamePad.GetState(controllingPlayer);
				break;
			}
		}
	}

	static Input()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		mouseDetected = false;
		keyboardState = Keyboard.GetState();
		keysPressedLastFrame = new List<Keys>();
		mouseWheelDelta = 0;
		controllingPlayer = (PlayerIndex)0;
	}
}
