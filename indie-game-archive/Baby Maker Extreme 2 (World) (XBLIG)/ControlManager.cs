using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
			if (KeyPressed(Keys.Space))
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
		if (m_iBounceCountdown > 0)
		{
			m_iBounceCountdown -= gameTime.ElapsedGameTime.Milliseconds;
		}
		m_prevKeyState = m_curKeyState;
		m_curKeyState = Keyboard.GetState();
		m_prevMouseState = m_curMouseState;
		m_curMouseState = Mouse.GetState();
		for (int i = 0; i < 4; i++)
		{
			ref GamePadState reference = ref m_prevPadState[i];
			reference = m_curPadState[i];
			ref GamePadState reference2 = ref m_curPadState[i];
			reference2 = GamePad.GetState(GetPlayerIndex(i), GamePadDeadZone.None);
			if (m_iDirectionLastPressed[i] == 0 && HoldingUp(i) > 0f)
			{
				m_iHeldTimer[i] += gameTime.ElapsedGameTime.Milliseconds;
				if (m_iHeldTimer[i] > m_iStartRepeatTime + m_iRepeatRate)
				{
					m_iForceDirectionPressed[i] = m_iDirectionLastPressed[i];
					m_iHeldTimer[i] -= m_iRepeatRate;
				}
				else
				{
					m_iForceDirectionPressed[i] = -1;
				}
			}
			else if (m_iDirectionLastPressed[i] == 1 && HoldingRight(i) > 0f)
			{
				m_iHeldTimer[i] += gameTime.ElapsedGameTime.Milliseconds;
				if (m_iHeldTimer[i] > m_iStartRepeatTime + m_iRepeatRate)
				{
					m_iForceDirectionPressed[i] = m_iDirectionLastPressed[i];
					m_iHeldTimer[i] -= m_iRepeatRate;
				}
				else
				{
					m_iForceDirectionPressed[i] = -1;
				}
			}
			else if (m_iDirectionLastPressed[i] == 2 && HoldingDown(i) > 0f)
			{
				m_iHeldTimer[i] += gameTime.ElapsedGameTime.Milliseconds;
				if (m_iHeldTimer[i] > m_iStartRepeatTime + m_iRepeatRate)
				{
					m_iForceDirectionPressed[i] = m_iDirectionLastPressed[i];
					m_iHeldTimer[i] -= m_iRepeatRate;
				}
				else
				{
					m_iForceDirectionPressed[i] = -1;
				}
			}
			else if (m_iDirectionLastPressed[i] == 3 && HoldingLeft(i) > 0f)
			{
				m_iHeldTimer[i] += gameTime.ElapsedGameTime.Milliseconds;
				if (m_iHeldTimer[i] > m_iStartRepeatTime + m_iRepeatRate)
				{
					m_iForceDirectionPressed[i] = m_iDirectionLastPressed[i];
					m_iHeldTimer[i] -= m_iRepeatRate;
				}
				else
				{
					m_iForceDirectionPressed[i] = -1;
				}
			}
			else
			{
				m_iDirectionLastPressed[i] = -1;
				m_iForceDirectionPressed[i] = -1;
			}
			if (ControlConn(i))
			{
				float num = m_Vibrations[i];
				if (num > 1f)
				{
					num = 1f;
				}
				if (rumbleEnabled && (GamePad.GetCapabilities(GetPlayerIndex(i)).HasLeftVibrationMotor || GamePad.GetCapabilities(GetPlayerIndex(i)).HasRightVibrationMotor))
				{
					GamePad.SetVibration(GetPlayerIndex(i), num, num);
				}
			}
			m_Vibrations[i] -= (float)gameTime.ElapsedGameTime.Milliseconds / 500f;
			if (m_Vibrations[i] < 0f)
			{
				m_Vibrations[i] = 0f;
			}
		}
	}

	public static void SetController(int i)
	{
		controllerType = i;
	}

	public static void Initialize()
	{
		m_iBounceCountdown = 0;
		controllerType = 2;
		m_iStartRepeatTime = 500;
		m_iRepeatRate = 200;
		m_bHasKeyboard = false;
		rumbleEnabled = true;
		activeMenuIndex = -1;
		m_curKeyState = Keyboard.GetState();
		m_prevKeyState = Keyboard.GetState();
		m_curPadState = new GamePadState[4];
		m_prevPadState = new GamePadState[4];
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
		return i switch
		{
			0 => PlayerIndex.One, 
			1 => PlayerIndex.Two, 
			2 => PlayerIndex.Three, 
			3 => PlayerIndex.Four, 
			_ => PlayerIndex.One, 
		};
	}

	public static bool ControlConn(int i)
	{
		if (i < 0)
		{
			return false;
		}
		return m_curPadState[i].IsConnected;
	}

	public static bool HoldingInstrumentButton(int i, int j)
	{
		if (controllerType == 1)
		{
			if (j == 0 && HoldingButton(i, Buttons.B))
			{
				return true;
			}
			if (j == 1 && HoldingButton(i, Buttons.Y))
			{
				return true;
			}
			if (j == 2 && HoldingButton(i, Buttons.X))
			{
				return true;
			}
			if (j == 3 && HoldingButton(i, Buttons.A))
			{
				return true;
			}
		}
		else if (controllerType == 0)
		{
			if (j == 0 && HoldingButton(i, Buttons.A))
			{
				return true;
			}
			if (j == 1 && HoldingButton(i, Buttons.B))
			{
				return true;
			}
			if (j == 2 && HoldingButton(i, Buttons.Y))
			{
				return true;
			}
			if (j == 3 && HoldingButton(i, Buttons.X))
			{
				return true;
			}
		}
		else if (controllerType == 3)
		{
			if (j == 3 && HoldingButton(i, Buttons.A))
			{
				return true;
			}
			if (j == 2 && HoldingButton(i, Buttons.B))
			{
				return true;
			}
			if (j == 1 && HoldingButton(i, Buttons.Y))
			{
				return true;
			}
			if (j == 0 && HoldingButton(i, Buttons.X))
			{
				return true;
			}
		}
		else if (controllerType == 2)
		{
			if (j == 0 && HoldingButton(i, Buttons.X))
			{
				return true;
			}
			if (j == 1 && HoldingButton(i, Buttons.Y))
			{
				return true;
			}
			if (j == 2 && HoldingButton(i, Buttons.A))
			{
				return true;
			}
			if (j == 3 && HoldingButton(i, Buttons.B))
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
		if (KeyPressed(Keys.D1))
		{
			return 0;
		}
		if (KeyPressed(Keys.D2))
		{
			return 1;
		}
		if (KeyPressed(Keys.D3))
		{
			return 2;
		}
		if (KeyPressed(Keys.D4))
		{
			return 3;
		}
		if (controllerType == 2)
		{
			if (PressedButton(i, Buttons.X))
			{
				return 0;
			}
			if (PressedButton(i, Buttons.Y))
			{
				return 1;
			}
			if (PressedButton(i, Buttons.A))
			{
				return 2;
			}
			if (PressedButton(i, Buttons.B))
			{
				return 3;
			}
		}
		else if (controllerType == 1)
		{
			if (m_iBounceCountdown <= 0)
			{
				if (PressedButton(i, Buttons.B))
				{
					m_iBounceCountdown = 200;
					return 0;
				}
				if (PressedButton(i, Buttons.Y))
				{
					m_iBounceCountdown = 200;
					return 1;
				}
				if (PressedButton(i, Buttons.X))
				{
					m_iBounceCountdown = 200;
					return 2;
				}
				if (PressedButton(i, Buttons.A))
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
			if (ControlConn(i) && (PressedButton(i, Buttons.DPadUp) || PressedButton(i, Buttons.DPadDown)))
			{
				if (HoldingButton(i, Buttons.A))
				{
					return Math.Abs(-num);
				}
				if (HoldingButton(i, Buttons.B))
				{
					return Math.Abs(1 - num);
				}
				if (HoldingButton(i, Buttons.Y))
				{
					return Math.Abs(2 - num);
				}
				if (HoldingButton(i, Buttons.X))
				{
					return Math.Abs(3 - num);
				}
			}
		}
		return -1;
	}

	public static bool HoldingButton(int i, Buttons b)
	{
		if (ControlConn(i))
		{
			return m_curPadState[i].IsButtonDown(b);
		}
		return false;
	}

	public static bool PressedButton(int i, Buttons b)
	{
		if (b == Buttons.B && KeyPressed(Keys.Back))
		{
			return true;
		}
		if (ControlConn(i) && m_curPadState[i].IsButtonDown(b))
		{
			return m_prevPadState[i].IsButtonUp(b);
		}
		return false;
	}

	public static bool PressedActivate(int i)
	{
		if (!KeyPressed(Keys.Space))
		{
			return PressedButton(i, Buttons.A);
		}
		return true;
	}

	public static bool PressedStart(int i)
	{
		if (!KeyPressed(Keys.Pause))
		{
			return PressedButton(i, Buttons.Start);
		}
		return true;
	}

	public static bool PressedUp(int i)
	{
		if (KeyPressed(Keys.Up) || PressedButton(i, Buttons.DPadUp) || (ControlConn(i) && m_curPadState[i].ThumbSticks.Left.Y > 0.5f && m_prevPadState[i].ThumbSticks.Left.Y < 0.5f))
		{
			m_iDirectionLastPressed[i] = 0;
			m_iHeldTimer[i] = 0;
			return true;
		}
		return m_iForceDirectionPressed[i] == 0;
	}

	public static bool PressedDown(int i)
	{
		if (KeyPressed(Keys.Down) || PressedButton(i, Buttons.DPadDown) || (ControlConn(i) && m_curPadState[i].ThumbSticks.Left.Y < -0.5f && m_prevPadState[i].ThumbSticks.Left.Y > -0.5f))
		{
			m_iDirectionLastPressed[i] = 2;
			m_iHeldTimer[i] = 0;
			return true;
		}
		return m_iForceDirectionPressed[i] == 2;
	}

	public static float HoldingUp(int i)
	{
		if (KeyHeld(Keys.Up) || HoldingButton(i, Buttons.DPadUp))
		{
			return 1f;
		}
		if (ControlConn(i) && m_curPadState[i].ThumbSticks.Left.Y > 0.25f)
		{
			return m_prevPadState[i].ThumbSticks.Left.Y;
		}
		return 0f;
	}

	public static float HoldingDown(int i)
	{
		if (KeyHeld(Keys.Down) || HoldingButton(i, Buttons.DPadDown))
		{
			return 1f;
		}
		if (ControlConn(i) && m_prevPadState[i].ThumbSticks.Left.Y < -0.25f)
		{
			return 0f - m_curPadState[i].ThumbSticks.Left.Y;
		}
		return 0f;
	}

	public static bool PressedLeft(int i)
	{
		if (KeyPressed(Keys.Left) || PressedButton(i, Buttons.DPadLeft) || (ControlConn(i) && m_curPadState[i].ThumbSticks.Left.X < -0.5f && m_prevPadState[i].ThumbSticks.Left.X > -0.5f))
		{
			m_iDirectionLastPressed[i] = 3;
			m_iHeldTimer[i] = 0;
			return true;
		}
		return m_iForceDirectionPressed[i] == 3;
	}

	public static bool PressedRight(int i)
	{
		if (KeyPressed(Keys.Right) || PressedButton(i, Buttons.DPadRight) || (ControlConn(i) && m_curPadState[i].ThumbSticks.Left.X > 0.5f && m_prevPadState[i].ThumbSticks.Left.X < 0.5f))
		{
			m_iDirectionLastPressed[i] = 1;
			m_iHeldTimer[i] = 0;
			return true;
		}
		return m_iForceDirectionPressed[i] == 1;
	}

	public static Vector2 RightStick(int i)
	{
		if (ControlConn(i) && (m_curPadState[i].ThumbSticks.Right.X < -0.25f || m_curPadState[i].ThumbSticks.Right.X > 0.25f || m_curPadState[i].ThumbSticks.Right.Y < -0.25f || m_curPadState[i].ThumbSticks.Right.Y > 0.25f))
		{
			return new Vector2(m_curPadState[i].ThumbSticks.Right.X, m_curPadState[i].ThumbSticks.Right.Y);
		}
		return default(Vector2);
	}

	public static Vector2 LeftStick(int i)
	{
		if (ControlConn(i) && (m_curPadState[i].ThumbSticks.Left.X < -0.25f || m_curPadState[i].ThumbSticks.Left.X > 0.25f || m_curPadState[i].ThumbSticks.Left.Y < -0.25f || m_curPadState[i].ThumbSticks.Left.Y > 0.25f))
		{
			return new Vector2(m_curPadState[i].ThumbSticks.Left.X, m_curPadState[i].ThumbSticks.Left.Y);
		}
		return default(Vector2);
	}

	public static float HoldingLeft(int i)
	{
		if (KeyHeld(Keys.Left) || HoldingButton(i, Buttons.DPadLeft))
		{
			return 1f;
		}
		if (ControlConn(i) && m_curPadState[i].ThumbSticks.Left.X < -0.25f)
		{
			return 0f - m_curPadState[i].ThumbSticks.Left.X;
		}
		return 0f;
	}

	public static float HoldingRight(int i)
	{
		if (KeyHeld(Keys.Right) || HoldingButton(i, Buttons.DPadRight))
		{
			return 1f;
		}
		if (ControlConn(i) && m_curPadState[i].ThumbSticks.Left.X > 0.25f)
		{
			return m_curPadState[i].ThumbSticks.Left.X;
		}
		return 0f;
	}

	public static bool PressedBackButton(int i)
	{
		if (!KeyPressed(Keys.Escape))
		{
			return PressedButton(i, Buttons.Back);
		}
		return true;
	}

	public static bool PressedDelete()
	{
		if (m_curKeyState.IsKeyDown(Keys.Delete))
		{
			return m_prevKeyState.IsKeyUp(Keys.Delete);
		}
		return false;
	}

	public static bool PressedLeftClick()
	{
		if (m_curMouseState.LeftButton == ButtonState.Pressed)
		{
			return m_prevMouseState.LeftButton == ButtonState.Released;
		}
		return false;
	}

	public static bool HoldingLeftClick()
	{
		return m_curMouseState.LeftButton == ButtonState.Pressed;
	}

	public static bool PressedRightClick()
	{
		if (m_curMouseState.RightButton == ButtonState.Pressed)
		{
			return m_prevMouseState.RightButton == ButtonState.Released;
		}
		return false;
	}

	public static bool HoldingRightClick()
	{
		return m_curMouseState.RightButton == ButtonState.Pressed;
	}

	public static Vector2 CursorPosition()
	{
		return new Vector2(m_curMouseState.X, m_curMouseState.Y);
	}

	public static Vector2 CursorPositionCamera()
	{
		return default(Vector2);
	}

	public static Vector2 CursorOffset()
	{
		return new Vector2(m_prevMouseState.X - m_curMouseState.X, m_prevMouseState.Y - m_curMouseState.Y);
	}

	public static bool KeyHeld(Keys k)
	{
		return m_curKeyState.IsKeyDown(k);
	}

	public static bool KeyPressed(Keys k)
	{
		if (m_curKeyState.IsKeyDown(k))
		{
			return m_prevKeyState.IsKeyUp(k);
		}
		return false;
	}

	public static bool WhammyHit(int i)
	{
		if (ControlConn(i) && m_curPadState[i].ThumbSticks.Right.X >= 0f && m_prevPadState[i].ThumbSticks.Right.X < 0f)
		{
			return true;
		}
		return false;
	}
}
