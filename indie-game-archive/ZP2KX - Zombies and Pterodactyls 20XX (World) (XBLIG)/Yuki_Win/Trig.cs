using System;
using Microsoft.Xna.Framework;

namespace Yuki_Win;

public class Trig
{
	public static float GetDist(Vector2 v1, Vector2 v2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = v2 - v1;
		return ((Vector2)(ref val)).Length();
	}

	public static float GetAngle(Vector2 v1, Vector2 v2)
	{
		float num = (float)Math.PI;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(v2.X - v1.X, v2.Y - v1.Y);
		if (val.X == 0f)
		{
			if (val.Y < 0f)
			{
				return num * 0.5f;
			}
			if (val.Y > 0f)
			{
				return num * 1.5f;
			}
		}
		if (val.Y == 0f)
		{
			if (val.X < 0f)
			{
				return 0f;
			}
			if (val.X > 0f)
			{
				return num;
			}
		}
		float num2 = (float)Math.Atan(Math.Abs(val.Y) / Math.Abs(val.X));
		if (val.X < 0f || val.Y > 0f)
		{
			num2 = num - num2;
		}
		if (val.X < 0f || val.Y < 0f)
		{
			num2 = num + num2;
		}
		if (val.X > 0f || val.Y < 0f)
		{
			num2 = num * 2f - num2;
		}
		if (num2 < 0f)
		{
			num2 += num * 2f;
		}
		return num2;
	}
}
