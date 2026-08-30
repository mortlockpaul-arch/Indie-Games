using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Billard3;

public class CollisionMobile
{
	public class Info
	{
		private bool value;

		private float time;

		private Point ballIDs;

		public bool Value => value;

		public float Time => time;

		public Point BallIDs => ballIDs;

		public Info()
		{
			time = -1f;
			ballIDs = new Point(-1, -1);
			value = false;
		}

		public Info(float t, Point b)
		{
			time = t;
			ballIDs = b;
			value = true;
		}
	}

	public static void PreCompute()
	{
		PreCompute(null, 1f);
	}

	public static void PreCompute(List<int> onlyForTheseBallNums, float maxTime)
	{
		for (int i = 0; i < 27; i++)
		{
			Ball ball = Statics.balls[i];
			for (int j = i + 1; j < 28; j++)
			{
				Ball ball2 = Statics.balls[j];
				if ((onlyForTheseBallNums == null || onlyForTheseBallNums.Contains(i) || onlyForTheseBallNums.Contains(j)) && ball.Alive && ball2.Alive && IsDistanceCritical(ball.Pos.Value2D, ball.Velo.Len, ball2.Pos.Value2D, ball2.Velo.Len))
				{
					Vector2 value2D = ball.Velo.Value2D;
					Vector2 value2D2 = ball2.Velo.Value2D;
					float num = TimeOfClosestApproach(ball.Pos.Value2D, ball2.Pos.Value2D, value2D, value2D2, 0.833333f, 0.833333f, out var collision);
					collision &= num >= 0f && num <= 1f;
					if (collision && num >= maxTime)
					{
						collision = false;
					}
					if (collision)
					{
						Updates.CollisionList.Add(new Updates.CollisionList.Item(num, i, j));
					}
				}
			}
		}
	}

	public static bool Reposition_And_Impulses(Info info)
	{
		Ball ball = Statics.balls[info.BallIDs.X];
		Ball ball2 = Statics.balls[info.BallIDs.Y];
		Vector2 p = ball.Pos.Value2D + info.Time * ball.Velo.Value2D;
		Vector2 p2 = ball2.Pos.Value2D + info.Time * ball2.Velo.Value2D;
		ball.Pos.Set(p);
		ball2.Pos.Set(p2);
		applyImpulsesVersionTownsend(ball.Number, ball.Pos.Value2D, ball.Velo.Value2D, ball.Velo.Len, ball2.Number, ball2.Pos.Value2D, ball2.Velo.Value2D, ball2.Velo.Len);
		return true;
	}

	private static void applyImpulsesVersionTownsend(int id1, Vector2 pos1, Vector2 vel1, float velLen1, int id2, Vector2 pos2, Vector2 vel2, float velLen2)
	{
		Vector2.Distance(pos1, pos2);
		Vector2 vector = Vector2.Normalize(pos1 - pos2);
		Vector2 vector2 = Vector2.Dot(vel1, -1f * vector) * (-1f * vector);
		vector2.Length();
		Vector2 vector3 = vel1 - vector2;
		vector3.Length();
		Vector2 vector4 = Vector2.Dot(vel2, vector) * vector;
		vector4.Length();
		Vector2 vector5 = vel2 - vector4;
		vector5.Length();
		(vector2 + vector3).Length();
		(vector4 + vector5).Length();
		Vector2 newVelo = vector3 + vector4;
		Vector2 newVelo2 = vector5 + vector2;
		Statics.balls[id1].ChangeVeloCollisionMobile(newVelo);
		Statics.balls[id2].ChangeVeloCollisionMobile(newVelo2);
	}

	private static bool IsDistanceCritical(Vector2 pos0, float veloLen0, Vector2 pos1, float veloLen1)
	{
		return Vector2.Distance(pos0, pos1) <= veloLen0 + veloLen1 + 1.7499993f;
	}

	public static float TimeOfClosestApproach(Vector2 Pa, Vector2 Pb, Vector2 Va, Vector2 Vb, float Ra, float Rb, out bool collision)
	{
		Vector2 vector = Pa - Pb;
		Vector2 vector2 = Va - Vb;
		float num = Vector2.Dot(vector2, vector2);
		float num2 = 2f * Vector2.Dot(vector, vector2);
		float num3 = Vector2.Dot(vector, vector) - (Ra + Rb) * (Ra + Rb);
		float num4 = num2 * num2 - 4f * num * num3;
		float num5;
		if (num4 < 0f)
		{
			num5 = (0f - num2) / (2f * num);
			collision = false;
		}
		else
		{
			float val = (0f - num2 + (float)Math.Sqrt(num4)) / (2f * num);
			float val2 = (0f - num2 - (float)Math.Sqrt(num4)) / (2f * num);
			num5 = Math.Min(val, val2);
			if (num5 < 0f)
			{
				collision = false;
			}
			else
			{
				collision = true;
			}
		}
		if (num5 < 0f)
		{
			num5 = 0f;
		}
		return num5;
	}
}
