using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ErrorReporting;

public class GameInput
{
	private Dictionary<string, Input> mInputs = new Dictionary<string, Input>();

	public Input MyInput(string theAction)
	{
		if (!mInputs.ContainsKey(theAction))
		{
			mInputs.Add(theAction, new Input());
		}
		return mInputs[theAction];
	}

	public void BeginUpdate()
	{
		Input.BeginUpdate();
	}

	public void EndUpdate()
	{
		Input.EndUpdate();
	}

	public bool IsConnected(PlayerIndex thePlayer)
	{
		if (!Input.GamepadConnectionState[thePlayer])
		{
			return true;
		}
		return Input.IsConnected(thePlayer);
	}

	public bool IsPressed(string theAction, PlayerIndex thePlayer)
	{
		if (!mInputs.ContainsKey(theAction))
		{
			return false;
		}
		return mInputs[theAction].IsPressed(thePlayer);
	}

	public bool IsPressed(string theAction, PlayerIndex? thePlayer)
	{
		PlayerIndex theControllingPlayer;
		if (!thePlayer.HasValue)
		{
			return IsPressed(theAction, thePlayer, out theControllingPlayer);
		}
		return IsPressed(theAction, thePlayer.Value);
	}

	public bool IsPressed(string theAction, PlayerIndex? thePlayer, out PlayerIndex theControllingPlayer)
	{
		if (!mInputs.ContainsKey(theAction))
		{
			theControllingPlayer = PlayerIndex.One;
			return false;
		}
		if (!thePlayer.HasValue)
		{
			if (IsPressed(theAction, PlayerIndex.One))
			{
				theControllingPlayer = PlayerIndex.One;
				return true;
			}
			if (IsPressed(theAction, PlayerIndex.Two))
			{
				theControllingPlayer = PlayerIndex.Two;
				return true;
			}
			if (IsPressed(theAction, PlayerIndex.Three))
			{
				theControllingPlayer = PlayerIndex.Three;
				return true;
			}
			if (IsPressed(theAction, PlayerIndex.Four))
			{
				theControllingPlayer = PlayerIndex.Four;
				return true;
			}
			theControllingPlayer = PlayerIndex.One;
			return false;
		}
		theControllingPlayer = thePlayer.Value;
		return IsPressed(theAction, thePlayer.Value);
	}

	public void AddGamePadInput(string theAction, Buttons theButton, bool isReleasedPreviously)
	{
		MyInput(theAction).AddGamepadInput(theButton, isReleasedPreviously);
	}

	public void AddKeyboardInput(string theAction, Keys theKey, bool isReleasedPreviously)
	{
		MyInput(theAction).AddKeyboardInput(theKey, isReleasedPreviously);
	}
}
