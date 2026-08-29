using Microsoft.Xna.Framework;

namespace GKEngine.Input;

public class InputAnalog2D
{
	public enum Type
	{
		Keyboard,
		Mouse,
		GamePad
	}

	public Type type;

	public KeyboardJoystick keyboard;

	public MouseButton mouse;

	public GamePadAnalog2D gamePad;

	public Vector2 value = new Vector2(-2f, -2f);

	public Vector2 previous = new Vector2(-2f, -2f);

	public int gamePadIndex;

	public InputAnalog2D(GamePadAnalog2D oAnalog)
	{
		gamePad = oAnalog;
		gamePadIndex = UniversalInput.gamePadPrimaryIndex;
		type = Type.GamePad;
	}

	public InputAnalog2D(GamePadAnalog2D oAnalog, int xGamePadIndex)
	{
		gamePad = oAnalog;
		gamePadIndex = xGamePadIndex;
		type = Type.GamePad;
	}

	public InputAnalog2D(KeyboardJoystick oKeyboardJoystick)
	{
		keyboard = oKeyboardJoystick;
		keyboard.active = true;
		type = Type.Keyboard;
	}

	public void Update(GameTime oGameTime)
	{
		if (type == Type.Keyboard)
		{
			keyboard.active = true;
			keyboard.Update(oGameTime);
			previous = value;
			value = keyboard.value;
		}
		else if (type == Type.GamePad)
		{
			if (UniversalInput.gamePadState[gamePadIndex].IsConnected)
			{
				previous = value;
				UniversalInput.GamePadAnalog2DValue(gamePad, gamePadIndex, ref value);
				return;
			}
			previous.X = -2f;
			previous.Y = -2f;
			value.X = -2f;
			value.Y = -2f;
		}
		else if (type == Type.Mouse)
		{
			previous.X = -2f;
			previous.Y = -2f;
			value.X = -2f;
			value.Y = -2f;
		}
	}
}
