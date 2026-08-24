using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AircraftRC;

public class InputState
{
	public int MaxInputs = 4;

	public GamePadState[] CurrentGamePadStates;

	public readonly GamePadState[] LastGamePadStates;

	public readonly bool[] GamePadWasConnected;

	public int player;

	public InputState()
	{
		CurrentGamePadStates = new GamePadState[MaxInputs];
		LastGamePadStates = new GamePadState[MaxInputs];
		GamePadWasConnected = new bool[MaxInputs];
	}

	public void Update()
	{
		for (int i = 0; i < MaxInputs; i = checked(i + 1))
		{
			ref GamePadState reference = ref LastGamePadStates[i];
			reference = CurrentGamePadStates[i];
			ref GamePadState reference2 = ref CurrentGamePadStates[i];
			reference2 = GamePad.GetState((PlayerIndex)i);
			player = i;
			if (CurrentGamePadStates[i].IsConnected)
			{
				GamePadWasConnected[i] = true;
			}
		}
	}

	public bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		if (controllingPlayer.HasValue)
		{
			playerIndex = controllingPlayer.Value;
			int num = (int)playerIndex;
			return CurrentGamePadStates[num].IsButtonDown(button);
		}
		if (!IsButtonPressed(button, PlayerIndex.One, out playerIndex) && !IsButtonPressed(button, PlayerIndex.Two, out playerIndex) && !IsButtonPressed(button, PlayerIndex.Three, out playerIndex))
		{
			return IsButtonPressed(button, PlayerIndex.Four, out playerIndex);
		}
		return true;
	}

	public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		if (controllingPlayer.HasValue)
		{
			playerIndex = controllingPlayer.Value;
			int num = (int)playerIndex;
			if (CurrentGamePadStates[num].IsButtonDown(button))
			{
				return LastGamePadStates[num].IsButtonUp(button);
			}
			return false;
		}
		if (!IsNewButtonPress(button, PlayerIndex.One, out playerIndex) && !IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) && !IsNewButtonPress(button, PlayerIndex.Three, out playerIndex))
		{
			return IsNewButtonPress(button, PlayerIndex.Four, out playerIndex);
		}
		return true;
	}
}
