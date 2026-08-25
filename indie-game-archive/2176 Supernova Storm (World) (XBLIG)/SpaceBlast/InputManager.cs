using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceBlast;

internal static class InputManager
{
	public static PlayerIndex? Player1Controller = null;

	public static PlayerIndex? Player2Controller = null;

	public static bool ListenForPlayer1Controller()
	{
		if (Player1Controller.HasValue)
		{
			return true;
		}
		for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
		{
			if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed)
			{
				Player1Controller = playerIndex;
				return true;
			}
		}
		return false;
	}

	public static bool ListenForPlayer2Controller()
	{
		if (Player2Controller.HasValue)
		{
			return true;
		}
		for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
		{
			if ((!Player1Controller.HasValue || Player1Controller.Value != playerIndex) && (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed))
			{
				Player2Controller = playerIndex;
				return true;
			}
		}
		return false;
	}

	public static GamePadState GetPlayer1Input()
	{
		if (!Player1Controller.HasValue)
		{
			return default(GamePadState);
		}
		return GamePad.GetState(Player1Controller.Value);
	}

	public static GamePadState GetPlayer2Input()
	{
		if (!Player2Controller.HasValue)
		{
			return default(GamePadState);
		}
		return GamePad.GetState(Player2Controller.Value);
	}

	public static void SetPlayer1Vibration(float leftMotor, float rightMotor)
	{
		GamePad.SetVibration(Player1Controller.Value, leftMotor, rightMotor);
	}

	public static void SetPlayer2Vibration(float leftMotor, float rightMotor)
	{
		GamePad.SetVibration(Player2Controller.Value, leftMotor, rightMotor);
	}
}
