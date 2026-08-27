using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class ControllerBase
{
	private struct ControllerData
	{
		public PlayerIndex pIndex;

		public float leftTimer;

		public float rightTimer;

		public float leftValue;

		public float rightValue;
	}

	private static bool initialized = false;

	private static ControllerData[] controllers = new ControllerData[4];

	private static void Initialize()
	{
		initialized = true;
		for (int i = 0; i < 4; i++)
		{
			controllers[i].pIndex = (PlayerIndex)i;
			controllers[i].leftTimer = 0f;
			controllers[i].rightTimer = 0f;
		}
	}

	public static void SetVibration(PlayerIndex pIndex, float leftTimer, float rightTimer, float leftValue, float rightValue)
	{
		controllers[(int)pIndex].leftTimer = leftTimer;
		controllers[(int)pIndex].leftValue = leftValue;
		controllers[(int)pIndex].rightTimer = rightTimer;
		controllers[(int)pIndex].rightValue = rightValue;
	}

	public static bool IsVibrating(PlayerIndex pIndex)
	{
		if (controllers[(int)pIndex].leftTimer > 0f || controllers[(int)pIndex].rightTimer > 0f)
		{
			return true;
		}
		return false;
	}

	public static bool HasSleeped(PlayerIndex pIndex, float time)
	{
		if (controllers[(int)pIndex].leftTimer < 0f - time && controllers[(int)pIndex].rightTimer < 0f - time)
		{
			return true;
		}
		return false;
	}

	public static void Update(GameTime gameTime)
	{
		float num = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
		if (!initialized)
		{
			Initialize();
		}
		for (int i = 0; i < 4; i++)
		{
			float leftMotor = 0f;
			float rightMotor = 0f;
			controllers[i].leftTimer -= num;
			if (controllers[i].leftTimer > 0f)
			{
				leftMotor = controllers[i].leftValue;
			}
			controllers[i].rightTimer -= num;
			if (controllers[i].rightTimer > 0f)
			{
				rightMotor = controllers[i].rightValue;
			}
			GamePad.SetVibration(controllers[i].pIndex, leftMotor, rightMotor);
		}
	}
}
