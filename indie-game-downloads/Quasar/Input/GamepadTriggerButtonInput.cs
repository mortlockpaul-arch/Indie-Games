using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.Input;

public class GamepadTriggerButtonInput : BaseInput<ButtonInputState>
{
	private Trigger trigger;

	public GamepadTriggerButtonInput(Trigger trigger)
	{
		this.trigger = trigger;
	}

	public void Update(List<PlayerIndex> playerIndices, ref ButtonInputState result)
	{
		PlayerIndex whoPressed = PlayerIndex.One;
		if (!result.Value.Value && Gamepad.TriggerStatus(playerIndices, trigger, ref whoPressed))
		{
			result.Value = new InputValue<bool>(value: true, whoPressed);
		}
	}

	public void Update(PlayerIndex playerIndex, ref ButtonInputState result)
	{
		if (!result.Value.Value && Gamepad.Instance(playerIndex).TriggerStatus(trigger))
		{
			result.Value = new InputValue<bool>(value: true, playerIndex);
		}
	}
}
