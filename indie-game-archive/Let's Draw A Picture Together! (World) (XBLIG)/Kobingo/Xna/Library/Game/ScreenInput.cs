using Kobingo.Xna.Library.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Kobingo.Xna.Library.Game;

public class ScreenInput
{
	public static bool Start
	{
		get
		{
			if (!KeyboardManager.IsKeyPress((Keys)32))
			{
				return GamepadManager.IsButtonPressed((Buttons)16);
			}
			return true;
		}
	}

	public static bool Pause
	{
		get
		{
			if (!KeyboardManager.IsKeyPress((Keys)27))
			{
				return GamepadManager.IsButtonPressed((Buttons)16);
			}
			return true;
		}
	}

	public static bool Select
	{
		get
		{
			if (!KeyboardManager.IsKeyPress((Keys)13))
			{
				return GamepadManager.IsButtonPressed((Buttons)4096);
			}
			return true;
		}
	}

	public static bool Back
	{
		get
		{
			if (!KeyboardManager.IsKeyPress((Keys)27) && !GamepadManager.IsButtonPressed((Buttons)32))
			{
				return GamepadManager.IsButtonPressed((Buttons)8192);
			}
			return true;
		}
	}

	public static bool Left
	{
		get
		{
			if (!KeyboardManager.IsKeyPress((Keys)37) && !GamepadManager.IsButtonPressed((Buttons)2097152))
			{
				return GamepadManager.IsButtonPressed((Buttons)4);
			}
			return true;
		}
	}

	public static bool Right
	{
		get
		{
			if (!KeyboardManager.IsKeyPress((Keys)39) && !GamepadManager.IsButtonPressed((Buttons)1073741824))
			{
				return GamepadManager.IsButtonPressed((Buttons)8);
			}
			return true;
		}
	}

	public static bool Up
	{
		get
		{
			if (!KeyboardManager.IsKeyPress((Keys)38) && !GamepadManager.IsButtonPressed((Buttons)268435456))
			{
				return GamepadManager.IsButtonPressed((Buttons)1);
			}
			return true;
		}
	}

	public static bool Down
	{
		get
		{
			if (!KeyboardManager.IsKeyPress((Keys)40) && !GamepadManager.IsButtonPressed((Buttons)536870912))
			{
				return GamepadManager.IsButtonPressed((Buttons)2);
			}
			return true;
		}
	}

	public static bool IsStart(PlayerIndex playerIndex)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!KeyboardManager.IsKeyPress(playerIndex, (Keys)32))
		{
			return GamepadManager.IsButtonPressed(playerIndex, (Buttons)16);
		}
		return true;
	}

	public static bool IsSelect(PlayerIndex playerIndex)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!KeyboardManager.IsKeyPress(playerIndex, (Keys)13))
		{
			return GamepadManager.IsButtonPressed(playerIndex, (Buttons)4096);
		}
		return true;
	}

	public static bool IsBack(PlayerIndex playerIndex)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!KeyboardManager.IsKeyPress(playerIndex, (Keys)27) && !GamepadManager.IsButtonPressed(playerIndex, (Buttons)32))
		{
			return GamepadManager.IsButtonPressed(playerIndex, (Buttons)8192);
		}
		return true;
	}
}
