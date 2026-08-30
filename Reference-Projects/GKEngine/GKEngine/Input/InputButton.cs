using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GKEngine.Input;

public class InputButton
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

	public GamePadButton button;

	public bool downed;

	public bool pressed;

	public int gamePadIndex;

	public bool isDown
	{
		get
		{
			bool result = false;
			if (type == Type.GamePad)
			{
				result = UniversalInput.GamePadButtonDown(button, gamePadIndex);
			}
			else if (type == Type.Keyboard)
			{
				result = UniversalInput.keyboardState.IsKeyDown(key);
			}
			return result;
		}
	}

	public InputButton(Keys oKey)
	{
		key = oKey;
		type = Type.Keyboard;
	}

	public InputButton(GamePadButton oButton)
	{
		button = oButton;
		gamePadIndex = UniversalInput.gamePadPrimaryIndex;
		type = Type.GamePad;
	}

	public InputButton(GamePadButton oButton, int xGamePadIndex)
	{
		button = oButton;
		gamePadIndex = xGamePadIndex;
		type = Type.GamePad;
	}

	public InputButton(MouseButton oMouse)
	{
		mouse = oMouse;
		type = Type.Mouse;
	}

	public void Update(GameTime oGameTime)
	{
		if (type == Type.Keyboard)
		{
			pressed = UniversalInput.KeyboardPressed(key);
		}
		else if (type == Type.GamePad)
		{
			pressed = UniversalInput.GamePadButtonPressed(button, gamePadIndex);
		}
		if (type == Type.Keyboard)
		{
			downed = UniversalInput.KeyboardDowned(key);
		}
		else if (type == Type.GamePad)
		{
			downed = UniversalInput.GamePadButtonDowned(button, gamePadIndex);
		}
	}
}
