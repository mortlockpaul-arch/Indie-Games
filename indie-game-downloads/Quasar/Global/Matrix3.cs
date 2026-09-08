using System;
using Microsoft.Xna.Framework;

namespace Quasar.Global;

public struct Matrix3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
{
	public float M11 = m11;

	public float M12 = m12;

	public float M13 = m13;

	public float M21 = m21;

	public float M22 = m22;

	public float M23 = m23;

	public float M31 = m31;

	public float M32 = m32;

	public float M33 = m33;

	private static Matrix3 identity;

	public Vector2 Translation => new Vector2(M31, M32);

	public float Rotation => (float)Math.Atan2(M12, M11);

	public Vector2 Scale => new Vector2(new Vector2(M11, M12).Length() * (float)Math.Sign(M11), new Vector2(M21, M22).Length() * (float)Math.Sign(M22));

	public static Matrix3 Identity => identity;

	public Vector2 Up => new Vector2(M21, M22);

	public Vector2 Right => new Vector2(M11, M12);

	static Matrix3()
	{
		identity = new Matrix3(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);
	}

	public static Matrix3 CreateRotation(float rotation)
	{
		float num = (float)Math.Cos(rotation);
		float num2 = (float)Math.Sin(rotation);
		return new Matrix3(num, num2, 0f, 0f - num2, num, 0f, 0f, 0f, 1f);
	}

	public static void CreateRotation(float rotation, out Matrix3 m)
	{
		float num = (float)Math.Cos(rotation);
		float num2 = (float)Math.Sin(rotation);
		m.M11 = num;
		m.M12 = num2;
		m.M13 = 0f;
		m.M21 = 0f - num2;
		m.M22 = num;
		m.M23 = 0f;
		m.M31 = 0f;
		m.M32 = 0f;
		m.M33 = 1f;
	}

	public static Matrix3 CreateScale(Vector2 scale)
	{
		return new Matrix3(scale.X, 0f, 0f, 0f, scale.Y, 0f, 0f, 0f, 1f);
	}

	public static void CreateScale(ref Vector2 scale, out Matrix3 m)
	{
		m.M11 = scale.X;
		m.M12 = 0f;
		m.M13 = 0f;
		m.M21 = 0f;
		m.M22 = scale.Y;
		m.M23 = 0f;
		m.M31 = 0f;
		m.M32 = 0f;
		m.M33 = 1f;
	}

	public static Matrix3 CreateTranslation(Vector2 translation)
	{
		return new Matrix3(1f, 0f, 0f, 0f, 1f, 0f, translation.X, translation.Y, 1f);
	}

	public static void CreateTranslation(ref Vector2 translation, out Matrix3 m)
	{
		m.M11 = 1f;
		m.M12 = 0f;
		m.M13 = 0f;
		m.M21 = 0f;
		m.M22 = 1f;
		m.M23 = 0f;
		m.M31 = translation.X;
		m.M32 = translation.Y;
		m.M33 = 1f;
	}

	public static void Multiply(ref Matrix3 m1, ref Matrix3 m2, out Matrix3 result)
	{
		float m3 = m1.M11 * m2.M11 + m1.M12 * m2.M21 + m1.M13 * m2.M31;
		float m4 = m1.M11 * m2.M12 + m1.M12 * m2.M22 + m1.M13 * m2.M32;
		float m5 = m1.M11 * m2.M13 + m1.M12 * m2.M23 + m1.M13 * m2.M33;
		float m6 = m1.M21 * m2.M11 + m1.M22 * m2.M21 + m1.M23 * m2.M31;
		float m7 = m1.M21 * m2.M12 + m1.M22 * m2.M22 + m1.M23 * m2.M32;
		float m8 = m1.M21 * m2.M13 + m1.M22 * m2.M23 + m1.M23 * m2.M33;
		float m9 = m1.M31 * m2.M11 + m1.M32 * m2.M21 + m1.M33 * m2.M31;
		float m10 = m1.M31 * m2.M12 + m1.M32 * m2.M22 + m1.M33 * m2.M32;
		float m11 = m1.M31 * m2.M13 + m1.M32 * m2.M23 + m1.M33 * m2.M33;
		result.M33 = m11;
		result.M32 = m10;
		result.M31 = m9;
		result.M23 = m8;
		result.M22 = m7;
		result.M21 = m6;
		result.M13 = m5;
		result.M12 = m4;
		result.M11 = m3;
	}

	public static Matrix3 operator *(Matrix3 m, Matrix3 m2)
	{
		Multiply(ref m, ref m2, out var result);
		return result;
	}

	public static void Transform(ref Vector2 v2, ref Matrix3 m3, out Vector2 result)
	{
		result = new Vector2(v2.X * m3.M11 + v2.Y * m3.M21 + m3.M31, v2.X * m3.M12 + v2.Y * m3.M22 + m3.M32);
	}
}
