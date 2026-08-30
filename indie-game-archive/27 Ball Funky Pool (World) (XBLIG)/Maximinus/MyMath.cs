using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class MyMath
{
	public struct Triangle
	{
		public readonly Vector3 V0;

		public readonly Vector3 V1;

		public readonly Vector3 V2;

		public readonly Vector3 Normal;

		public Triangle(Vector3 v0, Vector3 v1, Vector3 v2)
		{
			V0 = v0;
			V1 = v1;
			V2 = v2;
			Normal = Vector3.Normalize(Vector3.Cross(V0 - V1, V0 - V2));
		}

		public bool IntersectLine(Vector3 line0, Vector3 line1, out Vector3 I)
		{
			I = line0;
			Vector3 vector = V1 - V0;
			Vector3 vector2 = V2 - V0;
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			if (vector3 == Vector3.Zero)
			{
				return false;
			}
			Vector3 vector4 = line1 - line0;
			Vector3 vector5 = line0 - V0;
			float num = 0f - Vector3.Dot(vector3, vector5);
			float num2 = Vector3.Dot(vector3, vector4);
			if (Math.Abs(num2) < 1E-08f)
			{
				if (num == 0f)
				{
					return true;
				}
				return false;
			}
			float num3 = num / num2;
			I = line0 + num3 * vector4;
			float num4 = Vector3.Dot(vector, vector);
			float num5 = Vector3.Dot(vector, vector2);
			float num6 = Vector3.Dot(vector2, vector2);
			Vector3 vector6 = I - V0;
			float num7 = Vector3.Dot(vector6, vector);
			float num8 = Vector3.Dot(vector6, vector2);
			float num9 = num5 * num5 - num4 * num6;
			float num10 = (num5 * num8 - num6 * num7) / num9;
			if ((double)num10 < 0.0 || (double)num10 > 1.0)
			{
				return false;
			}
			float num11 = (num5 * num7 - num4 * num8) / num9;
			if ((double)num11 < 0.0 || (double)(num10 + num11) > 1.0)
			{
				return false;
			}
			return true;
		}
	}

	public struct Triangle2D(Vector2 a, Vector2 b, Vector2 c)
	{
		public Vector2 A = a;

		public Vector2 B = b;

		public Vector2 C = c;

		public bool Contains(Vector2 P)
		{
			Vector2 vector = C - A;
			Vector2 vector2 = B - A;
			Vector2 value = P - A;
			float num = Vector2.Dot(vector, vector);
			float num2 = Vector2.Dot(vector, vector2);
			float num3 = Vector2.Dot(vector, value);
			float num4 = Vector2.Dot(vector2, vector2);
			float num5 = Vector2.Dot(vector2, value);
			float num6 = 1f / (num * num4 - num2 * num2);
			float num7 = (num4 * num3 - num2 * num5) * num6;
			float num8 = (num * num5 - num2 * num3) * num6;
			if (num7 > 0f && num8 > 0f)
			{
				return num7 + num8 < 1f;
			}
			return false;
		}
	}

	public static Vector3 Project3D(Vector3 P0, Vector3 P1, Vector3 toProject)
	{
		if (P0.Equals(P1))
		{
			throw new ArgumentException("P1 and P2 must be different.");
		}
		Vector3 vector = P1 - P0;
		Vector3 vector2 = toProject - P0;
		Vector3 vector3 = vector * Vector3.Dot(vector, vector2) / Vector3.Dot(vector, vector);
		return P0 + vector3;
	}

	public static Vector2 Project2D(Vector2 P0, Vector2 P1, Vector2 toProject)
	{
		float num = (P1.Y - P0.Y) / (P1.X - P0.X);
		float num2 = P0.Y - num * P0.X;
		float x = (num * toProject.Y + toProject.X - num * num2) / (num * num + 1f);
		float y = (num * num * toProject.Y + num * toProject.X + num2) / (num * num + 1f);
		return new Vector2(x, y);
	}

	public static Vector3 CatMullRom3D(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float amount)
	{
		return new Vector3
		{
			X = MathHelper.CatmullRom(v1.X, v2.X, v3.X, v4.X, amount),
			Y = MathHelper.CatmullRom(v1.Y, v2.Y, v3.Y, v4.Y, amount),
			Z = MathHelper.CatmullRom(v1.Z, v2.Z, v3.Z, v4.Z, amount)
		};
	}

	public static List<Vector3> InterpolateCatMullRom(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int detailPerSegment)
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < detailPerSegment; i++)
		{
			Vector3 item = CatMullRom3D(v1, v2, v3, v4, (float)i / (float)detailPerSegment);
			list.Add(item);
		}
		return list;
	}

	public static Matrix MatrixRotationAxisCenter(Vector3 Center, Vector3 axis, float radians)
	{
		return Matrix.CreateTranslation(-Center) * Matrix.CreateFromAxisAngle(axis, radians) * Matrix.CreateTranslation(Center);
	}

	public static Matrix MatrixRotationXCenter(Vector3 Center, float radians)
	{
		return MatrixRotationAxisCenter(Center, Vector3.UnitX, radians);
	}

	public static Matrix MatrixRotationYCenter(Vector3 Center, float radians)
	{
		return MatrixRotationAxisCenter(Center, Vector3.UnitY, radians);
	}

	public static Matrix MatrixRotationZCenter(Vector3 Center, float radians)
	{
		return MatrixRotationAxisCenter(Center, Vector3.UnitZ, radians);
	}

	public static float SqrtN(float value, int inversePower)
	{
		float num = value;
		for (int i = 0; i < inversePower; i++)
		{
			num = (float)Math.Sqrt(num);
		}
		return num;
	}

	public static Vector2 LerpVector2(Vector2 v1, Vector2 v2, float amount)
	{
		return new Vector2(MathHelper.Lerp(v1.X, v2.X, amount), MathHelper.Lerp(v1.Y, v2.Y, amount));
	}

	public static Vector2 Vector2InvertY(Vector2 v)
	{
		return new Vector2(v.X, 0f - v.Y);
	}

	public static double AngleDegBetweenVectors(Vector2 v1, Vector2 v2)
	{
		double num = AngleRadFromVectorNorm(v1) - AngleRadFromVectorNorm(v2);
		return num * 360.0 / (Math.PI * 2.0);
	}

	public static double AngleRadFromVectorNorm(Vector2 vector)
	{
		double num = Math.Acos(vector.X);
		if (vector.Y < 0f)
		{
			num *= -1.0;
		}
		return num;
	}

	public static Vector2 VectorNormFromAngleRad(double angleRad)
	{
		Vector2 zero = Vector2.Zero;
		zero.X = (float)Math.Cos(angleRad);
		zero.Y = (float)Math.Sin(angleRad);
		return zero;
	}

	public static Vector2 Vector2Orthogonal(Vector2 v)
	{
		return new Vector2(v.Y, 0f - v.X);
	}

	public static Vector3 Vector3OrthY(Vector3 v)
	{
		return new Vector3(v.Z, v.Y, 0f - v.X);
	}

	public static Vector3 Vector3OrthZ(Vector3 v)
	{
		return new Vector3(v.Y, 0f - v.X, 0f - v.Z);
	}
}
