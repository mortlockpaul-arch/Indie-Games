using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GKEngine.Input;

public class InputAnalog1D
{
	public enum Type
	{
		Keyboard,
		Mouse,
		GamePad
	}

	public Type type;

	public Keys key;

	public MouseButton mouse;

	public GamePadAnalog1D gamePad;

	public float value = -1f;

	public float previous = -1f;

	public int gamePadIndex;

	public InputAnalog1D(GamePadAnalog1D oAnalog)
	{
		gamePad = oAnalog;
		gamePadIndex = UniversalInput.gamePadPrimaryIndex;
		type = Type.GamePad;
	}

	public InputAnalog1D(GamePadAnalog1D oAnalog, int xGamePadIndex)
	{
		gamePad = oAnalog;
		gamePadIndex = xGamePadIndex;
		type = Type.GamePad;
	}

	public InputAnalog1D()
	{
		type = Type.Mouse;
	}

	public void Update(GameTime oGameTime)
	{
		if (type == Type.Keyboard)
		{
			previous = value;
			value = -1f;
		}
		else if (type == Type.GamePad)
		{
			previous = value;
			value = UniversalInput.GamePadAnalog1DValue(gamePad, gamePadIndex);
		}
	}
}
