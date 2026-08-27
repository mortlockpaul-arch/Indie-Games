using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ErrorReporting;

public class Input
{
	private Dictionary<Keys, bool> mKeyboard = new Dictionary<Keys, bool>();

	private Dictionary<Buttons, bool> mGamepad = new Dictionary<Buttons, bool>();

	public static Dictionary<PlayerIndex, GamePadState> CurrentGamePadState = new Dictionary<PlayerIndex, GamePadState>();

	public static Dictionary<PlayerIndex, GamePadState> PreviousGamePadState = new Dictionary<PlayerIndex, GamePadState>();

	public static KeyboardState CurrentKeyboardState;

	public static KeyboardState PreviousKeyboardState;

	public static Dictionary<PlayerIndex, bool> GamepadConnectionState = new Dictionary<PlayerIndex, bool>();

	public Input()
	{
		if (CurrentGamePadState.Count == 0)
		{
			CurrentGamePadState.Add(PlayerIndex.One, GamePad.GetState(PlayerIndex.One));
			CurrentGamePadState.Add(PlayerIndex.Two, GamePad.GetState(PlayerIndex.Two));
			CurrentGamePadState.Add(PlayerIndex.Three, GamePad.GetState(PlayerIndex.Three));
			CurrentGamePadState.Add(PlayerIndex.Four, GamePad.GetState(PlayerIndex.Four));
			PreviousGamePadState.Add(PlayerIndex.One, GamePad.GetState(PlayerIndex.One));
			PreviousGamePadState.Add(PlayerIndex.Two, GamePad.GetState(PlayerIndex.Two));
			PreviousGamePadState.Add(PlayerIndex.Three, GamePad.GetState(PlayerIndex.Three));
			PreviousGamePadState.Add(PlayerIndex.Four, GamePad.GetState(PlayerIndex.Four));
			GamepadConnectionState.Add(PlayerIndex.One, CurrentGamePadState[PlayerIndex.One].IsConnected);
			GamepadConnectionState.Add(PlayerIndex.Two, CurrentGamePadState[PlayerIndex.Two].IsConnected);
			GamepadConnectionState.Add(PlayerIndex.Three, CurrentGamePadState[PlayerIndex.Three].IsConnected);
			GamepadConnectionState.Add(PlayerIndex.Four, CurrentGamePadState[PlayerIndex.Four].IsConnected);
		}
	}

	public static void BeginUpdate()
	{
		CurrentGamePadState[PlayerIndex.One] = GamePad.GetState(PlayerIndex.One);
		CurrentGamePadState[PlayerIndex.Two] = GamePad.GetState(PlayerIndex.Two);
		CurrentGamePadState[PlayerIndex.Three] = GamePad.GetState(PlayerIndex.Three);
		CurrentGamePadState[PlayerIndex.Four] = GamePad.GetState(PlayerIndex.Four);
		CurrentKeyboardState = Keyboard.GetState(PlayerIndex.One);
	}

	public static void EndUpdate()
	{
		PreviousGamePadState[PlayerIndex.One] = CurrentGamePadState[PlayerIndex.One];
		PreviousGamePadState[PlayerIndex.Two] = CurrentGamePadState[PlayerIndex.Two];
		PreviousGamePadState[PlayerIndex.Three] = CurrentGamePadState[PlayerIndex.Three];
		PreviousGamePadState[PlayerIndex.Four] = CurrentGamePadState[PlayerIndex.Four];
		PreviousKeyboardState = CurrentKeyboardState;
	}

	public void AddKeyboardInput(Keys theKey, bool isReleasedPreviously)
	{
		if (mKeyboard.ContainsKey(theKey))
		{
			mKeyboard[theKey] = isReleasedPreviously;
		}
		else
		{
			mKeyboard.Add(theKey, isReleasedPreviously);
		}
	}

	public void AddGamepadInput(Buttons theButton, bool isReleasedPreviously)
	{
		if (mGamepad.ContainsKey(theButton))
		{
			mGamepad[theButton] = isReleasedPreviously;
		}
		else
		{
			mGamepad.Add(theButton, isReleasedPreviously);
		}
	}

	public static bool IsConnected(PlayerIndex thePlayerIndex)
	{
		return CurrentGamePadState[thePlayerIndex].IsConnected;
	}

	public bool IsPressed(PlayerIndex thePlayerIndex)
	{
		foreach (Keys key in mKeyboard.Keys)
		{
			if (mKeyboard[key])
			{
				if (CurrentKeyboardState.IsKeyDown(key) && !PreviousKeyboardState.IsKeyDown(key))
				{
					return true;
				}
			}
			else if (CurrentKeyboardState.IsKeyDown(key))
			{
				return true;
			}
		}
		foreach (Buttons key2 in mGamepad.Keys)
		{
			if (mGamepad[key2])
			{
				if (CurrentGamePadState[thePlayerIndex].IsButtonDown(key2) && !PreviousGamePadState[thePlayerIndex].IsButtonDown(key2))
				{
					return true;
				}
			}
			else if (CurrentGamePadState[thePlayerIndex].IsButtonDown(key2))
			{
				return true;
			}
		}
		return false;
	}
}
