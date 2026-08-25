using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RenegadeEngine;

public class Vibration
{
	private static TimeSpan[] vibrationDurations = new TimeSpan[4]
	{
		TimeSpan.Zero,
		TimeSpan.Zero,
		TimeSpan.Zero,
		TimeSpan.Zero
	};

	private static float[] leftMotors;

	private static float[] rightMotors;

	public static void StartVibration(int playerIndex, float duration, float leftMotorStrength, float rightMotorStrength)
	{
		ref TimeSpan reference = ref vibrationDurations[playerIndex];
		reference = TimeSpan.FromSeconds(duration);
		leftMotors[playerIndex] = leftMotorStrength;
		rightMotors[playerIndex] = 1f;
		switch (playerIndex)
		{
		case 0:
			GamePad.SetVibration(PlayerIndex.One, leftMotors[playerIndex], rightMotors[playerIndex]);
			break;
		case 1:
			GamePad.SetVibration(PlayerIndex.Two, leftMotors[playerIndex], rightMotors[playerIndex]);
			break;
		case 2:
			GamePad.SetVibration(PlayerIndex.Three, leftMotors[playerIndex], rightMotors[playerIndex]);
			break;
		case 3:
			GamePad.SetVibration(PlayerIndex.Four, leftMotors[playerIndex], rightMotors[playerIndex]);
			break;
		}
	}

	public static void Update(GameTime gameTime)
	{
		for (int i = 0; i < 4; i++)
		{
			if (vibrationDurations[i] > TimeSpan.Zero)
			{
				vibrationDurations[i] -= gameTime.ElapsedGameTime;
			}
			else if (leftMotors[i] > 0f || rightMotors[i] > 0f)
			{
				leftMotors[i] = 0f;
				rightMotors[i] = 0f;
				switch (i)
				{
				case 0:
					GamePad.SetVibration(PlayerIndex.One, leftMotors[i], rightMotors[i]);
					break;
				case 1:
					GamePad.SetVibration(PlayerIndex.Two, leftMotors[i], rightMotors[i]);
					break;
				case 2:
					GamePad.SetVibration(PlayerIndex.Three, leftMotors[i], rightMotors[i]);
					break;
				case 3:
					GamePad.SetVibration(PlayerIndex.Four, leftMotors[i], rightMotors[i]);
					break;
				}
			}
		}
	}

	public static void StopAllVibrations()
	{
		GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
		GamePad.SetVibration(PlayerIndex.Two, 0f, 0f);
		GamePad.SetVibration(PlayerIndex.Three, 0f, 0f);
		GamePad.SetVibration(PlayerIndex.Four, 0f, 0f);
	}

	static Vibration()
	{
		float[] array = new float[4];
		leftMotors = array;
		float[] array2 = new float[4];
		rightMotors = array2;
	}
}
