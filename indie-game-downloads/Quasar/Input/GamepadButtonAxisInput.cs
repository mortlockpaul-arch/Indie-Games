using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Quasar.Input;

public class GamepadButtonAxisInput : BaseInput<AxisInputState>
{
	private Buttons buttonDown;

	private Buttons buttonUp;

	public GamepadButtonAxisInput(Buttons downButton, Buttons upButton)
	{
		buttonDown = downButton;
		buttonUp = upButton;
	}

	public void Update(List<PlayerIndex> playerIndices, ref AxisInputState result)
	{
		PlayerIndex whoPressed = PlayerIndex.One;
		if (Gamepad.ButtonStatus(playerIndices, buttonDown, ref whoPressed))
		{
			result.Value.Value += -1f;
			result.Value.Player = whoPressed;
		}
		if (Gamepad.ButtonStatus(playerIndices, buttonUp, ref whoPressed))
		{
			result.Value.Value++;
			result.Value.Player = whoPressed;
		}
	}

	public void Update(PlayerIndex playerIndex, ref AxisInputState result)
	{
		if (Gamepad.Instance(playerIndex).ButtonStatus(buttonDown))
		{
			result.Value.Value += -1f;
			result.Value.Player = playerIndex;
		}
		if (Gamepad.Instance(playerIndex).ButtonStatus(buttonUp))
		{
			result.Value.Value++;
			result.Value.Player = playerIndex;
		}
	}
}
