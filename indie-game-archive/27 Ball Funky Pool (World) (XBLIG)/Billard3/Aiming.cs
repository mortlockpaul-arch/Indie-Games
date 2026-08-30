using System;
using Microsoft.Xna.Framework;

namespace Billard3;

public class Aiming
{
	private const float maxSpeed = 0.05f;

	private static double angle;

	public static float AngleRad => (float)angle;

	public static Vector3 AimVector => AimVectorStatic(angle);

	public static Vector2 AimVector2D => new VectorBillard(AimVector).Value2D;

	public static double DirectionToAngle(Vector2 dir)
	{
		Vector2 vector = Vector2.Normalize(dir);
		double num = Math.Acos(vector.X);
		if (vector.Y < 0f)
		{
			num = Math.PI * 2.0 - num;
		}
		return num;
	}

	public static void Change(float value)
	{
		angle += value;
	}

	public static void Set(float value)
	{
		angle = value;
	}

	public static void Initialize()
	{
		Ball ball = Statics.balls[0];
		Vector2 dir = Vector2.Zero - ball.Pos.Value2D;
		angle = DirectionToAngle(dir);
	}

	public static Vector3 AimVectorStatic(double staticAngle)
	{
		Vector3 result = new Vector3((float)Math.Cos(staticAngle), 0f, (float)Math.Sin(staticAngle));
		if (Math.Abs(result.Z) == 1f)
		{
			result.X = 0f;
		}
		if (Math.Abs(result.X) == 1f)
		{
			result.Z = 0f;
		}
		return result;
	}
}
