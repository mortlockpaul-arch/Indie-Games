using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Quasar.Input;

public class KeyboardTriggerInput : BaseInput<TriggerInputState>
{
	private Keys key;

	public KeyboardTriggerInput(Keys key)
	{
		this.key = key;
	}

	public void Update(List<PlayerIndex> playerIndices, ref TriggerInputState result)
	{
		Update(PlayerIndex.One, ref result);
	}

	public void Update(PlayerIndex playerIndex, ref TriggerInputState result)
	{
		if (result.Value.Value == 0f && Keyboard.Instance.KeyState(key))
		{
			result.Value = new InputValue<float>(1f, playerIndex);
		}
	}
}
