using System;
using Microsoft.Xna.Framework;

namespace Hammer;

public class Math2
{
	public static float TurnToFace(Vector2 position, Vector2 faceThis, float currentAngle, float turnSpeed)
	{
		float num = faceThis.X - position.X;
		float num2 = faceThis.Y - position.Y;
		float num3 = (float)Math.Atan2(num2, num);
		float value = MathHelper.WrapAngle(num3 - currentAngle);
		value = MathHelper.Clamp(value, 0f - turnSpeed, turnSpeed);
		return MathHelper.WrapAngle(currentAngle + value);
	}

	public static Vector2 AdvanceAngle(float angle, float distance)
	{
		return AdvanceAngle(Vector2.Zero, angle, distance);
	}

	public static Vector2 AdvanceAngle(Vector2 position, float angle, float distance)
	{
		return new Vector2(position.X + (float)(Math.Cos(angle) * (double)distance), position.Y + (float)(Math.Sin(angle) * (double)distance));
	}
}
