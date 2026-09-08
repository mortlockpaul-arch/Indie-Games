using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Quasar.Input;

public class GamepadButtonInput : BaseInput<ButtonInputState>
{
	private Buttons button;

	public GamepadButtonInput(Buttons button)
	{
		this.button = button;
	}

	public void Update(List<PlayerIndex> playerIndices, ref ButtonInputState result)
	{
		PlayerIndex whoPressed = PlayerIndex.One;
		if (!result.Value.Value && Gamepad.ButtonStatus(playerIndices, button, ref whoPressed))
		{
			result.Value = new InputValue<bool>(value: true, whoPressed);
		}
	}

	public void Update(PlayerIndex playerIndex, ref ButtonInputState result)
	{
		if (!result.Value.Value && Gamepad.Instance(playerIndex).ButtonStatus(button))
		{
			result.Value = new InputValue<bool>(value: true, playerIndex);
		}
	}
}
