using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Quasar.Input;

public class GamepadButtonAxis2Input : BaseInput<Axis2InputState>
{
	private Buttons buttonLeft;

	private Buttons buttonRight;

	private Buttons buttonDown;

	private Buttons buttonUp;

	public GamepadButtonAxis2Input(Buttons leftButton, Buttons rightButton, Buttons downButton, Buttons upButton)
	{
		buttonUp = upButton;
		buttonLeft = leftButton;
		buttonDown = downButton;
		buttonRight = rightButton;
	}

	public void Update(List<PlayerIndex> playerIndices, ref Axis2InputState result)
	{
		PlayerIndex whoPressed = PlayerIndex.One;
		if (Gamepad.ButtonStatus(playerIndices, buttonLeft, ref whoPressed))
		{
			result.Value.Value.X += -1f;
			result.Value.Player = whoPressed;
		}
		if (Gamepad.ButtonStatus(playerIndices, buttonRight, ref whoPressed))
		{
			result.Value.Value.X++;
			result.Value.Player = whoPressed;
		}
		if (Gamepad.ButtonStatus(playerIndices, buttonDown, ref whoPressed))
		{
			result.Value.Value.Y += -1f;
			result.Value.Player = whoPressed;
		}
		if (Gamepad.ButtonStatus(playerIndices, buttonUp, ref whoPressed))
		{
			result.Value.Value.Y++;
			result.Value.Player = whoPressed;
		}
	}

	public void Update(PlayerIndex playerIndex, ref Axis2InputState result)
	{
		if (Gamepad.Instance(playerIndex).ButtonStatus(buttonLeft))
		{
			result.Value.Value.X += -1f;
			result.Value.Player = playerIndex;
		}
		if (Gamepad.Instance(playerIndex).ButtonStatus(buttonRight))
		{
			result.Value.Value.X++;
			result.Value.Player = playerIndex;
		}
		if (Gamepad.Instance(playerIndex).ButtonStatus(buttonDown))
		{
			result.Value.Value.Y += -1f;
			result.Value.Player = playerIndex;
		}
		if (Gamepad.Instance(playerIndex).ButtonStatus(buttonUp))
		{
			result.Value.Value.Y++;
			result.Value.Player = playerIndex;
		}
	}
}
