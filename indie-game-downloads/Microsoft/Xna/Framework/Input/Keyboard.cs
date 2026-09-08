namespace Microsoft.Xna.Framework.Input;

public static class Keyboard
{
	internal static KeyboardState keys;

	public static KeyboardState GetState()
	{
		return keys;
	}

	public static KeyboardState GetState(PlayerIndex playerIndex)
	{
		return keys;
	}

	public static Keys GetKeyFromScancodeEXT(Keys scancode)
	{
		return FNAPlatform.GetKeyFromScancode(scancode);
	}
}
