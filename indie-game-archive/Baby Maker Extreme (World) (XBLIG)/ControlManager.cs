using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Renderer;

public class ControlManager
{
	private const int BounceCooldown = 200;

	private const float THRESH = 0.25f;

	private static KeyboardState m_curKeyState;

	private static KeyboardState m_prevKeyState;

	private static GamePadState[] m_curPadState;

	private static GamePadState[] m_prevPadState;

	private static MouseState m_curMouseState;

	private static MouseState m_prevMouseState;

	private static int activeMenuIndex;

	private static float[] m_Vibrations;

	private static bool m_bHasKeyboard;

	private static bool rumbleEnabled;

	private static int m_iStartRepeatTime;

	private static int m_iRepeatRate;

	private static int[] m_iDirectionLastPressed;

	private static int[] m_iForceDirectionPressed;

	private static int[] m_iHeldTimer;

	private static int controllerType;

	private static int m_iBounceCountdown;

	public static int ActiveMenuIndex
	{
		get
		{
			return activeMenuIndex;
		}
		set
		{
			activeMenuIndex = value;
			if (KeyPressed((Keys)32))
			{
				m_bHasKeyboard = true;
			}
		}
	}

	public static bool HasKeyboard => m_bHasKeyboard;

	public static void SetRumble(int i)
	{
		rumbleEnabled = i == 0;
	}

	public static bool IsRumbleEnabled()
	{
		return rumbleEnabled;
	}

	public static void UpdateInput(GameTime gameTime)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		if (m_iBounceCountdown > 0)
		{
			m_iBounceCountdown -= gameTime.ElapsedGameTime.Milliseconds;
		}
		m_prevKeyState = m_curKeyState;
		m_curKeyState = Keyboard.GetState();
		m_prevMouseState = m_curMouseState;
		m_curMouseState = Mouse.GetState();
		for (int num = 0; num < 4; num++)
		{
			ref GamePadState reference = ref m_prevPadState[num];
			reference = m_curPadState[num];
			ref GamePadState reference2 = ref m_curPadState[num];
			reference2 = GamePad.GetState(GetPlayerIndex(num), (GamePadDeadZone)0);
			if (m_iDirectionLastPressed[num] == 0 && HoldingUp(num) > 0f)
			{
				m_iHeldTimer[num] += gameTime.ElapsedGameTime.Milliseconds;
				if (m_iHeldTimer[num] > m_iStartRepeatTime + m_iRepeatRate)
				{
					m_iForceDirectionPressed[num] = m_iDirectionLastPressed[num];
					m_iHeldTimer[num] -= m_iRepeatRate;
				}
				else
				{
					m_iForceDirectionPressed[num] = -1;
				}
			}
			else if (m_iDirectionLastPressed[num] == 1 && HoldingRight(num) > 0f)
			{
				m_iHeldTimer[num] += gameTime.ElapsedGameTime.Milliseconds;
				if (m_iHeldTimer[num] > m_iStartRepeatTime + m_iRepeatRate)
				{
					m_iForceDirectionPressed[num] = m_iDirectionLastPressed[num];
					m_iHeldTimer[num] -= m_iRepeatRate;
				}
				else
				{
					m_iForceDirectionPressed[num] = -1;
				}
			}
			else if (m_iDirectionLastPressed[num] == 2 && HoldingDown(num) > 0f)
			{
				m_iHeldTimer[num] += gameTime.ElapsedGameTime.Milliseconds;
				if (m_iHeldTimer[num] > m_iStartRepeatTime + m_iRepeatRate)
				{
					m_iForceDirectionPressed[num] = m_iDirectionLastPressed[num];
					m_iHeldTimer[num] -= m_iRepeatRate;
				}
				else
				{
					m_iForceDirectionPressed[num] = -1;
				}
			}
			else if (m_iDirectionLastPressed[num] == 3 && HoldingLeft(num) > 0f)
			{
				m_iHeldTimer[num] += gameTime.ElapsedGameTime.Milliseconds;
				if (m_iHeldTimer[num] > m_iStartRepeatTime + m_iRepeatRate)
				{
					m_iForceDirectionPressed[num] = m_iDirectionLastPressed[num];
					m_iHeldTimer[num] -= m_iRepeatRate;
				}
				else
				{
					m_iForceDirectionPressed[num] = -1;
				}
			}
			else
			{
				m_iDirectionLastPressed[num] = -1;
				m_iForceDirectionPressed[num] = -1;
			}
			if (ControlConn(num))
			{
				float num2 = m_Vibrations[num];
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				if (rumbleEnabled)
				{
					GamePadCapabilities capabilities = GamePad.GetCapabilities(GetPlayerIndex(num));
					if (!((GamePadCapabilities)(ref capabilities)).HasLeftVibrationMotor)
					{
						GamePadCapabilities capabilities2 = GamePad.GetCapabilities(GetPlayerIndex(num));
						if (!((GamePadCapabilities)(ref capabilities2)).HasRightVibrationMotor)
						{
							goto IL_0330;
						}
					}
					GamePad.SetVibration(GetPlayerIndex(num), num2, num2);
				}
			}
			goto IL_0330;
			IL_0330:
			m_Vibrations[num] -= (float)gameTime.ElapsedGameTime.Milliseconds / 500f;
			if (m_Vibrations[num] < 0f)
			{
				m_Vibrations[num] = 0f;
			}
		}
	}

	public static void SetController(int i)
	{
		controllerType = i;
	}

	public static void Initialize()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		m_iBounceCountdown = 0;
		controllerType = 2;
		m_iStartRepeatTime = 500;
		m_iRepeatRate = 200;
		m_bHasKeyboard = false;
		rumbleEnabled = true;
		activeMenuIndex = -1;
		m_curKeyState = Keyboard.GetState();
		m_prevKeyState = Keyboard.GetState();
		m_curPadState = (GamePadState[])(object)new GamePadState[4];
		m_prevPadState = (GamePadState[])(object)new GamePadState[4];
		m_curMouseState = Mouse.GetState();
		m_prevMouseState = Mouse.GetState();
		m_Vibrations = new float[4];
		m_iHeldTimer = new int[4];
		m_iDirectionLastPressed = new int[4];
		m_iForceDirectionPressed = new int[4];
		for (int i = 0; i < 4; i++)
		{
			ref GamePadState reference = ref m_curPadState[i];
			reference = GamePad.GetState(GetPlayerIndex(i));
			ref GamePadState reference2 = ref m_prevPadState[i];
			reference2 = GamePad.GetState(GetPlayerIndex(i));
			m_iDirectionLastPressed[i] = -1;
			m_iForceDirectionPressed[i] = -1;
			m_iHeldTimer[i] = 0;
			m_Vibrations[i] = 0f;
		}
	}

	public static void SetVibration(int i, float f)
	{
		f = Math.Abs(f);
		if (i >= 0 && i < 4)
		{
			m_Vibrations[i] += f * 3f;
			if (m_Vibrations[i] > 2f)
			{
				m_Vibrations[i] = 2f;
			}
		}
	}

	public static void SetFlatVibration(int i, float f)
	{
		f = Math.Abs(f);
		if (i >= 0 && i < 4)
		{
			m_Vibrations[i] = f;
			if (m_Vibrations[i] > 2f)
			{
				m_Vibrations[i] = 2f;
			}
		}
	}

	public static int DetectInput()
	{
		for (int i = 0; i < 4; i++)
		{
			if (PressedActivate(i))
			{
				return i;
			}
			if (PressedStart(i))
			{
				return i;
			}
		}
		return -1;
	}

	public static int DetectStart()
	{
		for (int i = 0; i < 4; i++)
		{
			if (PressedStart(i))
			{
				return i;
			}
		}
		return -1;
	}

	public static PlayerIndex GetPlayerIndex(int i)
	{
		return (PlayerIndex)(i switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			3 => 3, 
			_ => 0, 
		});
	}

	public static bool ControlConn(int i)
	{
		if (i < 0)
		{
			return false;
		}
		return ((GamePadState)(ref m_curPadState[i])).IsConnected;
	}

	public static bool HoldingInstrumentButton(int i, int j)
	{
		if (controllerType == 1)
		{
			if (j == 0 && HoldingButton(i, (Buttons)8192))
			{
				return true;
			}
			if (j == 1 && HoldingButton(i, (Buttons)32768))
			{
				return true;
			}
			if (j == 2 && HoldingButton(i, (Buttons)16384))
			{
				return true;
			}
			if (j == 3 && HoldingButton(i, (Buttons)4096))
			{
				return true;
			}
		}
		else if (controllerType == 0)
		{
			if (j == 0 && HoldingButton(i, (Buttons)4096))
			{
				return true;
			}
			if (j == 1 && HoldingButton(i, (Buttons)8192))
			{
				return true;
			}
			if (j == 2 && HoldingButton(i, (Buttons)32768))
			{
				return true;
			}
			if (j == 3 && HoldingButton(i, (Buttons)16384))
			{
				return true;
			}
		}
		else if (controllerType == 3)
		{
			if (j == 3 && HoldingButton(i, (Buttons)4096))
			{
				return true;
			}
			if (j == 2 && HoldingButton(i, (Buttons)8192))
			{
				return true;
			}
			if (j == 1 && HoldingButton(i, (Buttons)32768))
			{
				return true;
			}
			if (j == 0 && HoldingButton(i, (Buttons)16384))
			{
				return true;
			}
		}
		else if (controllerType == 2)
		{
			if (j == 0 && HoldingButton(i, (Buttons)16384))
			{
				return true;
			}
			if (j == 1 && HoldingButton(i, (Buttons)32768))
			{
				return true;
			}
			if (j == 2 && HoldingButton(i, (Buttons)4096))
			{
				return true;
			}
			if (j == 3 && HoldingButton(i, (Buttons)8192))
			{
				return true;
			}
		}
		return false;
	}

	public static int ControllerType()
	{
		return controllerType;
	}

	public static int GetControlKeyIndex(int i)
	{
		if (KeyPressed((Keys)49))
		{
			return 0;
		}
		if (KeyPressed((Keys)50))
		{
			return 1;
		}
		if (KeyPressed((Keys)51))
		{
			return 2;
		}
		if (KeyPressed((Keys)52))
		{
			return 3;
		}
		if (controllerType == 2)
		{
			if (PressedButton(i, (Buttons)16384))
			{
				return 0;
			}
			if (PressedButton(i, (Buttons)32768))
			{
				return 1;
			}
			if (PressedButton(i, (Buttons)4096))
			{
				return 2;
			}
			if (PressedButton(i, (Buttons)8192))
			{
				return 3;
			}
		}
		else if (controllerType == 1)
		{
			if (m_iBounceCountdown <= 0)
			{
				if (PressedButton(i, (Buttons)8192))
				{
					m_iBounceCountdown = 200;
					return 0;
				}
				if (PressedButton(i, (Buttons)32768))
				{
					m_iBounceCountdown = 200;
					return 1;
				}
				if (PressedButton(i, (Buttons)16384))
				{
					m_iBounceCountdown = 200;
					return 2;
				}
				if (PressedButton(i, (Buttons)4096))
				{
					m_iBounceCountdown = 200;
					return 3;
				}
			}
		}
		else if (controllerType == 0 || controllerType == 3)
		{
			int num = 0;
			if (controllerType == 3)
			{
				num = 3;
			}
			if (ControlConn(i) && (PressedButton(i, (Buttons)1) || PressedButton(i, (Buttons)2)))
			{
				if (HoldingButton(i, (Buttons)4096))
				{
					return Math.Abs(-num);
				}
				if (HoldingButton(i, (Buttons)8192))
				{
					return Math.Abs(1 - num);
				}
				if (HoldingButton(i, (Buttons)32768))
				{
					return Math.Abs(2 - num);
				}
				if (HoldingButton(i, (Buttons)16384))
				{
					return Math.Abs(3 - num);
				}
			}
		}
		return -1;
	}

	public static bool HoldingButton(int i, Buttons b)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (ControlConn(i))
		{
			return ((GamePadState)(ref m_curPadState[i])).IsButtonDown(b);
		}
		return false;
	}

	public static bool PressedButton(int i, Buttons b)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if ((int)b == 8192 && KeyPressed((Keys)8))
		{
			return true;
		}
		if (ControlConn(i) && ((GamePadState)(ref m_curPadState[i])).IsButtonDown(b))
		{
			return ((GamePadState)(ref m_prevPadState[i])).IsButtonUp(b);
		}
		return false;
	}

	public static bool PressedActivate(int i)
	{
		if (!KeyPressed((Keys)32))
		{
			return PressedButton(i, (Buttons)4096);
		}
		return true;
	}

	public static bool PressedStart(int i)
	{
		if (!KeyPressed((Keys)19))
		{
			return PressedButton(i, (Buttons)16);
		}
		return true;
	}

	public static bool PressedUp(int i)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!KeyPressed((Keys)38) && !PressedButton(i, (Buttons)1))
		{
			if (ControlConn(i))
			{
				GamePadThumbSticks thumbSticks = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
				if (((GamePadThumbSticks)(ref thumbSticks)).Left.Y > 0.5f)
				{
					GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref m_prevPadState[i])).ThumbSticks;
					if (((GamePadThumbSticks)(ref thumbSticks2)).Left.Y < 0.5f)
					{
						goto IL_0062;
					}
				}
			}
			return m_iForceDirectionPressed[i] == 0;
		}
		goto IL_0062;
		IL_0062:
		m_iDirectionLastPressed[i] = 0;
		m_iHeldTimer[i] = 0;
		return true;
	}

	public static bool PressedDown(int i)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!KeyPressed((Keys)40) && !PressedButton(i, (Buttons)2))
		{
			if (ControlConn(i))
			{
				GamePadThumbSticks thumbSticks = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
				if (((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.5f)
				{
					GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref m_prevPadState[i])).ThumbSticks;
					if (((GamePadThumbSticks)(ref thumbSticks2)).Left.Y > -0.5f)
					{
						goto IL_0062;
					}
				}
			}
			return m_iForceDirectionPressed[i] == 2;
		}
		goto IL_0062;
		IL_0062:
		m_iDirectionLastPressed[i] = 2;
		m_iHeldTimer[i] = 0;
		return true;
	}

	public static float HoldingUp(int i)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (KeyHeld((Keys)38) || HoldingButton(i, (Buttons)1))
		{
			return 1f;
		}
		if (ControlConn(i))
		{
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks)).Left.Y > 0.25f)
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref m_prevPadState[i])).ThumbSticks;
				return ((GamePadThumbSticks)(ref thumbSticks2)).Left.Y;
			}
		}
		return 0f;
	}

	public static float HoldingDown(int i)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (KeyHeld((Keys)40) || HoldingButton(i, (Buttons)2))
		{
			return 1f;
		}
		if (ControlConn(i))
		{
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref m_prevPadState[i])).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.25f)
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
				return 0f - ((GamePadThumbSticks)(ref thumbSticks2)).Left.Y;
			}
		}
		return 0f;
	}

	public static bool PressedLeft(int i)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!KeyPressed((Keys)37) && !PressedButton(i, (Buttons)4))
		{
			if (ControlConn(i))
			{
				GamePadThumbSticks thumbSticks = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
				if (((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.5f)
				{
					GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref m_prevPadState[i])).ThumbSticks;
					if (((GamePadThumbSticks)(ref thumbSticks2)).Left.X > -0.5f)
					{
						goto IL_0062;
					}
				}
			}
			return m_iForceDirectionPressed[i] == 3;
		}
		goto IL_0062;
		IL_0062:
		m_iDirectionLastPressed[i] = 3;
		m_iHeldTimer[i] = 0;
		return true;
	}

	public static bool PressedRight(int i)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!KeyPressed((Keys)39) && !PressedButton(i, (Buttons)8))
		{
			if (ControlConn(i))
			{
				GamePadThumbSticks thumbSticks = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
				if (((GamePadThumbSticks)(ref thumbSticks)).Left.X > 0.5f)
				{
					GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref m_prevPadState[i])).ThumbSticks;
					if (((GamePadThumbSticks)(ref thumbSticks2)).Left.X < 0.5f)
					{
						goto IL_0062;
					}
				}
			}
			return m_iForceDirectionPressed[i] == 1;
		}
		goto IL_0062;
		IL_0062:
		m_iDirectionLastPressed[i] = 1;
		m_iHeldTimer[i] = 0;
		return true;
	}

	public static Vector2 RightStick(int i)
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (ControlConn(i))
		{
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks)).Right.X < -0.25f))
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks2)).Right.X > 0.25f))
				{
					GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
					if (!(((GamePadThumbSticks)(ref thumbSticks3)).Right.Y < -0.25f))
					{
						GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
						if (!(((GamePadThumbSticks)(ref thumbSticks4)).Right.Y > 0.25f))
						{
							goto IL_00dd;
						}
					}
				}
			}
			GamePadThumbSticks thumbSticks5 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
			float x = ((GamePadThumbSticks)(ref thumbSticks5)).Right.X;
			GamePadThumbSticks thumbSticks6 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
			return new Vector2(x, ((GamePadThumbSticks)(ref thumbSticks6)).Right.Y);
		}
		goto IL_00dd;
		IL_00dd:
		return default(Vector2);
	}

	public static Vector2 LeftStick(int i)
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (ControlConn(i))
		{
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.25f))
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks2)).Left.X > 0.25f))
				{
					GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
					if (!(((GamePadThumbSticks)(ref thumbSticks3)).Left.Y < -0.25f))
					{
						GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
						if (!(((GamePadThumbSticks)(ref thumbSticks4)).Left.Y > 0.25f))
						{
							goto IL_00dd;
						}
					}
				}
			}
			GamePadThumbSticks thumbSticks5 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
			float x = ((GamePadThumbSticks)(ref thumbSticks5)).Left.X;
			GamePadThumbSticks thumbSticks6 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
			return new Vector2(x, ((GamePadThumbSticks)(ref thumbSticks6)).Left.Y);
		}
		goto IL_00dd;
		IL_00dd:
		return default(Vector2);
	}

	public static float HoldingLeft(int i)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (KeyHeld((Keys)37) || HoldingButton(i, (Buttons)4))
		{
			return 1f;
		}
		if (ControlConn(i))
		{
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.25f)
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
				return 0f - ((GamePadThumbSticks)(ref thumbSticks2)).Left.X;
			}
		}
		return 0f;
	}

	public static float HoldingRight(int i)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (KeyHeld((Keys)39) || HoldingButton(i, (Buttons)8))
		{
			return 1f;
		}
		if (ControlConn(i))
		{
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks)).Left.X > 0.25f)
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
				return ((GamePadThumbSticks)(ref thumbSticks2)).Left.X;
			}
		}
		return 0f;
	}

	public static bool PressedBackButton(int i)
	{
		if (!KeyPressed((Keys)27))
		{
			return PressedButton(i, (Buttons)32);
		}
		return true;
	}

	public static bool PressedDelete()
	{
		if (((KeyboardState)(ref m_curKeyState)).IsKeyDown((Keys)46))
		{
			return ((KeyboardState)(ref m_prevKeyState)).IsKeyUp((Keys)46);
		}
		return false;
	}

	public static bool PressedLeftClick()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		if ((int)((MouseState)(ref m_curMouseState)).LeftButton == 1)
		{
			return (int)((MouseState)(ref m_prevMouseState)).LeftButton == 0;
		}
		return false;
	}

	public static bool HoldingLeftClick()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		return (int)((MouseState)(ref m_curMouseState)).LeftButton == 1;
	}

	public static bool PressedRightClick()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		if ((int)((MouseState)(ref m_curMouseState)).RightButton == 1)
		{
			return (int)((MouseState)(ref m_prevMouseState)).RightButton == 0;
		}
		return false;
	}

	public static bool HoldingRightClick()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		return (int)((MouseState)(ref m_curMouseState)).RightButton == 1;
	}

	public static Vector2 CursorPosition()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)((MouseState)(ref m_curMouseState)).X, (float)((MouseState)(ref m_curMouseState)).Y);
	}

	public static Vector2 CursorPositionCamera()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = SceneRenderer.GetCameraPosition() - SceneRenderer.GetViewportDim() / 2f / SceneRenderer.GetCameraZoom();
		return val + new Vector2((float)((MouseState)(ref m_curMouseState)).X, (float)((MouseState)(ref m_curMouseState)).Y) / SceneRenderer.GetCameraZoom();
	}

	public static Vector2 CursorOffset()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)(((MouseState)(ref m_prevMouseState)).X - ((MouseState)(ref m_curMouseState)).X), (float)(((MouseState)(ref m_prevMouseState)).Y - ((MouseState)(ref m_curMouseState)).Y));
	}

	public static bool KeyHeld(Keys k)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return ((KeyboardState)(ref m_curKeyState)).IsKeyDown(k);
	}

	public static bool KeyPressed(Keys k)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (((KeyboardState)(ref m_curKeyState)).IsKeyDown(k))
		{
			return ((KeyboardState)(ref m_prevKeyState)).IsKeyUp(k);
		}
		return false;
	}

	public static bool WhammyHit(int i)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (ControlConn(i))
		{
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref m_curPadState[i])).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks)).Right.X >= 0f)
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref m_prevPadState[i])).ThumbSticks;
				if (((GamePadThumbSticks)(ref thumbSticks2)).Right.X < 0f)
				{
					return true;
				}
			}
		}
		return false;
	}
}
