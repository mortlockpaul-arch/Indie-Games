using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Quasar.Input;

public class KeyboardButtonInput : BaseInput<ButtonInputState>
{
	private Keys key;

	public KeyboardButtonInput(Keys key)
	{
		this.key = key;
	}

	public void Update(List<PlayerIndex> playerIndices, ref ButtonInputState result)
	{
		Update(PlayerIndex.One, ref result);
	}

	public void Update(PlayerIndex playerIndex, ref ButtonInputState result)
	{
		if (!result.Value.Value && Keyboard.Instance.KeyState(key))
		{
			result.Value = new InputValue<bool>(value: true, playerIndex);
		}
	}
}
