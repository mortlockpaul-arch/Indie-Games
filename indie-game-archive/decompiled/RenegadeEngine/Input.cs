using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RenegadeEngine;

public static class Input
{
	private const int maxPlayers = 4;

	private static InputDevice[] playerAssignedDevices;

	public static bool[] DeviceDisconnected;

	private static KeyboardState[] prevKS;

	private static KeyboardState[] currKS;

	private static GamePadState[] currGPS;

	private static GamePadState[] prevGPS;

	public static KeyboardState GetCurrKS(PlayerIndex index)
	{
		return currKS[(int)index];
	}

	public static KeyboardState GetPrevKS(PlayerIndex index)
	{
		return prevKS[(int)index];
	}

	public static bool KeyPR(PlayerIndex index, Keys key)
	{
		if (prevKS[(int)index].IsKeyDown(key) && currKS[(int)index].IsKeyUp(key))
		{
			return true;
		}
		return false;
	}

	public static bool KeyRP(PlayerIndex index, Keys key)
	{
		if (prevKS[(int)index].IsKeyUp(key) && currKS[(int)index].IsKeyDown(key))
		{
			return true;
		}
		return false;
	}

	public static bool KeyPressed(PlayerIndex index, Keys key)
	{
		if (currKS[(int)index].IsKeyDown(key))
		{
			return true;
		}
		return false;
	}

	public static void ClearKeyboardState(PlayerIndex index)
	{
		currKS[(int)index] = default(KeyboardState);
		ref KeyboardState reference = ref prevKS[(int)index];
		reference = currKS[(int)index];
	}

	public static void ClearAllKeyboardStates()
	{
		for (int i = 0; i < 4; i++)
		{
			currKS[i] = default(KeyboardState);
			ref KeyboardState reference = ref prevKS[i];
			reference = currKS[i];
		}
	}

	public static GamePadState GetCurrGPS(PlayerIndex playerIndex)
	{
		return currGPS[(int)playerIndex];
	}

	public static GamePadState GetPrevGPS(PlayerIndex playerIndex)
	{
		return prevGPS[(int)playerIndex];
	}

	public static bool ButtonPR(PlayerIndex playerIndex, Buttons button)
	{
		if (currGPS[(int)playerIndex].IsButtonUp(button) && prevGPS[(int)playerIndex].IsButtonDown(button))
		{
			return true;
		}
		return false;
	}

	public static bool ButtonRP(PlayerIndex playerIndex, Buttons button)
	{
		if (currGPS[(int)playerIndex].IsButtonDown(button) && prevGPS[(int)playerIndex].IsButtonUp(button))
		{
			return true;
		}
		return false;
	}

	public static Vector2 LeftStick(PlayerIndex playerIndex)
	{
		return currGPS[(int)playerIndex].ThumbSticks.Left;
	}

	public static Vector2 RightStick(PlayerIndex playerIndex)
	{
		return currGPS[(int)playerIndex].ThumbSticks.Right;
	}

	public static bool LStickMenuUp(PlayerIndex playerIndex)
	{
		if (currGPS[(int)playerIndex].ThumbSticks.Left.Y > 0f && prevGPS[(int)playerIndex].ThumbSticks.Left.Y == 0f)
		{
			return true;
		}
		return false;
	}

	public static bool LStickMenuDown(PlayerIndex playerIndex)
	{
		if (currGPS[(int)playerIndex].ThumbSticks.Left.Y < 0f && prevGPS[(int)playerIndex].ThumbSticks.Left.Y == 0f)
		{
			return true;
		}
		return false;
	}

	public static bool LStickMenuRight(PlayerIndex playerIndex)
	{
		if (currGPS[(int)playerIndex].ThumbSticks.Left.X > 0f && prevGPS[(int)playerIndex].ThumbSticks.Left.X == 0f)
		{
			return true;
		}
		return false;
	}

	public static bool LStickMenuLeft(PlayerIndex playerIndex)
	{
		if (currGPS[(int)playerIndex].ThumbSticks.Left.X < 0f && prevGPS[(int)playerIndex].ThumbSticks.Left.X == 0f)
		{
			return true;
		}
		return false;
	}

	public static void ClearGamepadState(PlayerIndex playerIndex)
	{
		currGPS[(int)playerIndex] = default(GamePadState);
		ref GamePadState reference = ref prevGPS[(int)playerIndex];
		reference = currGPS[(int)playerIndex];
	}

	public static void ClearAllGamepadStates()
	{
		for (int i = 0; i < 4; i++)
		{
			currGPS[i] = default(GamePadState);
			ref GamePadState reference = ref prevGPS[i];
			reference = currGPS[i];
		}
	}

	public static void Begin()
	{
		ref KeyboardState reference = ref currKS[0];
		reference = Keyboard.GetState(PlayerIndex.One);
		ref KeyboardState reference2 = ref currKS[1];
		reference2 = Keyboard.GetState(PlayerIndex.Two);
		ref KeyboardState reference3 = ref currKS[2];
		reference3 = Keyboard.GetState(PlayerIndex.Three);
		ref KeyboardState reference4 = ref currKS[3];
		reference4 = Keyboard.GetState(PlayerIndex.Four);
		ref GamePadState reference5 = ref currGPS[0];
		reference5 = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
		ref GamePadState reference6 = ref currGPS[1];
		reference6 = GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular);
		ref GamePadState reference7 = ref currGPS[2];
		reference7 = GamePad.GetState(PlayerIndex.Three, GamePadDeadZone.Circular);
		ref GamePadState reference8 = ref currGPS[3];
		reference8 = GamePad.GetState(PlayerIndex.Four, GamePadDeadZone.Circular);
		DeviceDisconnected[0] = prevGPS[0].IsConnected && !currGPS[0].IsConnected;
		DeviceDisconnected[1] = prevGPS[1].IsConnected && !currGPS[1].IsConnected;
		DeviceDisconnected[2] = prevGPS[2].IsConnected && !currGPS[2].IsConnected;
		DeviceDisconnected[3] = prevGPS[3].IsConnected && !currGPS[3].IsConnected;
	}

	public static void End()
	{
		ref KeyboardState reference = ref prevKS[0];
		reference = currKS[0];
		ref KeyboardState reference2 = ref prevKS[1];
		reference2 = currKS[1];
		ref KeyboardState reference3 = ref prevKS[2];
		reference3 = currKS[2];
		ref KeyboardState reference4 = ref prevKS[3];
		reference4 = currKS[3];
		ref GamePadState reference5 = ref prevGPS[0];
		reference5 = currGPS[0];
		ref GamePadState reference6 = ref prevGPS[1];
		reference6 = currGPS[1];
		ref GamePadState reference7 = ref prevGPS[2];
		reference7 = currGPS[2];
		ref GamePadState reference8 = ref prevGPS[3];
		reference8 = currGPS[3];
	}

	public static void ClearAllInputStates()
	{
		ClearAllGamepadStates();
		ClearAllKeyboardStates();
	}

	public static bool MenuSelect(PlayerIndex playerIndex)
	{
		if (ButtonRP(playerIndex, Buttons.A) || ButtonRP(playerIndex, Buttons.Start) || KeyRP(playerIndex, Keys.Space) || KeyRP(playerIndex, Keys.Enter))
		{
			return true;
		}
		return false;
	}

	public static bool MenuCancel(PlayerIndex playerIndex)
	{
		if (ButtonRP(playerIndex, Buttons.B) || ButtonRP(playerIndex, Buttons.Back) || KeyRP(playerIndex, Keys.Back) || KeyRP(playerIndex, Keys.Escape))
		{
			return true;
		}
		return false;
	}

	public static bool MenuUp(PlayerIndex playerIndex)
	{
		if (ButtonPR(playerIndex, Buttons.DPadUp) || LStickMenuUp(playerIndex) || KeyPR(playerIndex, Keys.W) || KeyPR(playerIndex, Keys.Up))
		{
			return true;
		}
		return false;
	}

	public static bool MenuDown(PlayerIndex playerIndex)
	{
		if (ButtonPR(playerIndex, Buttons.DPadDown) || LStickMenuDown(playerIndex) || KeyPR(playerIndex, Keys.S) || KeyPR(playerIndex, Keys.Down))
		{
			return true;
		}
		return false;
	}

	public static bool MenuRight(PlayerIndex playerIndex)
	{
		if (ButtonPR(playerIndex, Buttons.DPadRight) || LStickMenuRight(playerIndex) || KeyPR(playerIndex, Keys.D) || KeyPR(playerIndex, Keys.Right))
		{
			return true;
		}
		return false;
	}

	public static bool MenuLeft(PlayerIndex playerIndex)
	{
		if (ButtonPR(playerIndex, Buttons.DPadLeft) || LStickMenuLeft(playerIndex) || KeyPR(playerIndex, Keys.A) || KeyPR(playerIndex, Keys.Left))
		{
			return true;
		}
		return false;
	}

	public static bool MenuQuit(PlayerIndex playerIndex)
	{
		if (ButtonPR(playerIndex, Buttons.B) || ButtonPR(playerIndex, Buttons.Back) || KeyPR(playerIndex, Keys.Escape))
		{
			ClearGamepadState(playerIndex);
			ClearKeyboardState(playerIndex);
			return true;
		}
		return false;
	}

	public static bool Pause(PlayerIndex playerIndex)
	{
		if ((currGPS[(int)playerIndex].IsButtonUp(Buttons.Start) && prevGPS[(int)playerIndex].IsButtonDown(Buttons.Start)) || (currKS[(int)playerIndex].IsKeyUp(Keys.Pause) && prevKS[(int)playerIndex].IsKeyDown(Keys.Pause)))
		{
			return true;
		}
		return false;
	}

	public static bool GameplayQuit(PlayerIndex playerIndex)
	{
		if ((currGPS[(int)playerIndex].IsButtonUp(Buttons.Back) && prevGPS[(int)playerIndex].IsButtonDown(Buttons.Back)) || (currKS[(int)playerIndex].IsKeyUp(Keys.Escape) && prevKS[(int)playerIndex].IsKeyDown(Keys.Escape)))
		{
			return true;
		}
		return false;
	}

	static Input()
	{
		InputDevice[] array = new InputDevice[4];
		playerAssignedDevices = array;
		bool[] deviceDisconnected = new bool[4];
		DeviceDisconnected = deviceDisconnected;
		prevKS = new KeyboardState[4];
		currKS = new KeyboardState[4];
		currGPS = new GamePadState[4];
		prevGPS = new GamePadState[4];
	}
}
