using System;
using Microsoft.Xna.Framework;

namespace Kobingo.Xna.Library.Common;

public static class VectorHelper
{
	public static Vector2 GetVector2(this Vector3 value)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(value.X, value.Y);
	}

	public static Vector3 GetVector3(this Vector2 value)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(value.X, value.Y, 0f);
	}

	public static Vector2 GetDirection(float angle)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.Normalize(new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)));
	}

	public static float GetAngle(Vector2 direction)
	{
		return (float)Math.Atan2(direction.Y, direction.X);
	}

	public static float GetAngle(Vector2 source, Vector2 direction)
	{
		return (float)Math.Atan2(direction.Y - source.Y, direction.X - source.X);
	}
}
