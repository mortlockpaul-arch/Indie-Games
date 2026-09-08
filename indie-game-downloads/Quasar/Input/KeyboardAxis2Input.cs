using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Quasar.Input;

public class KeyboardAxis2Input : BaseInput<Axis2InputState>
{
	private Keys keyLeft;

	private Keys keyRight;

	private Keys keyDown;

	private Keys keyUp;

	public KeyboardAxis2Input(Keys keyLeft, Keys keyRight, Keys keyDown, Keys keyUp)
	{
		this.keyLeft = keyLeft;
		this.keyRight = keyRight;
		this.keyDown = keyDown;
		this.keyUp = keyUp;
	}

	public void Update(List<PlayerIndex> playerIndices, ref Axis2InputState result)
	{
		Update(PlayerIndex.One, ref result);
	}

	public void Update(PlayerIndex playerIndex, ref Axis2InputState result)
	{
		if (Keyboard.Instance.KeyState(keyLeft))
		{
			result.Value.Value.X += -1f;
			result.Value.Player = playerIndex;
		}
		if (Keyboard.Instance.KeyState(keyRight))
		{
			result.Value.Value.X++;
			result.Value.Player = playerIndex;
		}
		if (Keyboard.Instance.KeyState(keyDown))
		{
			result.Value.Value.Y += -1f;
			result.Value.Player = playerIndex;
		}
		if (Keyboard.Instance.KeyState(keyUp))
		{
			result.Value.Value.Y++;
			result.Value.Player = playerIndex;
		}
	}
}
