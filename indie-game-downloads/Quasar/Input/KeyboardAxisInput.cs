using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Quasar.Input;

public class KeyboardAxisInput : BaseInput<AxisInputState>
{
	private Keys keyDown;

	private Keys keyUp;

	public KeyboardAxisInput(Keys keyDown, Keys keyUp)
	{
		this.keyDown = keyDown;
		this.keyUp = keyUp;
	}

	public void Update(List<PlayerIndex> playerIndices, ref AxisInputState result)
	{
		Update(PlayerIndex.One, ref result);
	}

	public void Update(PlayerIndex playerIndex, ref AxisInputState result)
	{
		if (Keyboard.Instance.KeyState(keyDown))
		{
			result.Value.Value += -1f;
			result.Value.Player = playerIndex;
		}
		if (Keyboard.Instance.KeyState(keyUp))
		{
			result.Value.Value++;
			result.Value.Player = playerIndex;
		}
	}
}
