using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceBlast;

internal static class InputManager
{
	public static PlayerIndex? Player1Controller = null;

	public static PlayerIndex? Player2Controller = null;

	public static bool ListenForPlayer1Controller()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (Player1Controller.HasValue)
		{
			return true;
		}
		PlayerIndex val = (PlayerIndex)0;
		while ((int)val <= 3)
		{
			GamePadState state = GamePad.GetState(val);
			GamePadButtons buttons = ((GamePadState)(ref state)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start != 1)
			{
				GamePadState state2 = GamePad.GetState(val);
				GamePadButtons buttons2 = ((GamePadState)(ref state2)).Buttons;
				if ((int)((GamePadButtons)(ref buttons2)).A != 1)
				{
					val = (PlayerIndex)(val + 1);
					continue;
				}
			}
			Player1Controller = val;
			return true;
		}
		return false;
	}

	public static bool ListenForPlayer2Controller()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Invalid comparison between Unknown and I4
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Invalid comparison between Unknown and I4
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (Player2Controller.HasValue)
		{
			return true;
		}
		for (PlayerIndex val = (PlayerIndex)0; (int)val <= 3; val = (PlayerIndex)(val + 1))
		{
			if (Player1Controller.HasValue && Player1Controller.Value == val)
			{
				continue;
			}
			GamePadState state = GamePad.GetState(val);
			GamePadButtons buttons = ((GamePadState)(ref state)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start != 1)
			{
				GamePadState state2 = GamePad.GetState(val);
				GamePadButtons buttons2 = ((GamePadState)(ref state2)).Buttons;
				if ((int)((GamePadButtons)(ref buttons2)).A != 1)
				{
					continue;
				}
			}
			Player2Controller = val;
			return true;
		}
		return false;
	}

	public static GamePadState GetPlayer1Input()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!Player1Controller.HasValue)
		{
			return default(GamePadState);
		}
		return GamePad.GetState(Player1Controller.Value);
	}

	public static GamePadState GetPlayer2Input()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!Player2Controller.HasValue)
		{
			return default(GamePadState);
		}
		return GamePad.GetState(Player2Controller.Value);
	}

	public static void SetPlayer1Vibration(float leftMotor, float rightMotor)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		GamePad.SetVibration(Player1Controller.Value, leftMotor, rightMotor);
	}

	public static void SetPlayer2Vibration(float leftMotor, float rightMotor)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		GamePad.SetVibration(Player2Controller.Value, leftMotor, rightMotor);
	}
}
