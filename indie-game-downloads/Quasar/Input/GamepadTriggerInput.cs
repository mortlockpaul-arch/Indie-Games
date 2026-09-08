using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.Input;

public class GamepadTriggerInput : BaseInput<TriggerInputState>
{
	private Trigger trigger;

	public GamepadTriggerInput(Trigger trigger)
	{
		this.trigger = trigger;
	}

	public void Update(List<PlayerIndex> playerIndices, ref TriggerInputState result)
	{
		result.Value.Value += Gamepad.SumTriggerPositions(playerIndices, trigger, ref result.Value.Player);
	}

	public void Update(PlayerIndex playerIndex, ref TriggerInputState result)
	{
		Gamepad gamepad = Gamepad.Instance(playerIndex);
		float num = gamepad.TriggerPosition(trigger);
		result.Value.Value += num;
		if (num != 0f)
		{
			result.Value.Player = playerIndex;
		}
	}
}
