using System;

namespace Microsoft.XboxLive.MathUtilities;

public struct Matrix
{
	public float M11;

	public float M12;

	public float M13;

	public float M14;

	public float M21;

	public float M22;

	public float M23;

	public float M24;

	public float M31;

	public float M32;

	public float M33;

	public float M34;

	public float M41;

	public float M42;

	public float M43;

	public float M44;

	public static Matrix Identity => new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);

	public Vector3 Translation
	{
		get
		{
			return new Vector3(M41, M42, M43);
		}
		set
		{
			M41 = value.X;
			M42 = value.Y;
			M43 = value.Z;
		}
	}

	public Matrix(float component11, float component12, float component13, float component14, float component21, float component22, float component23, float component24, float component31, float component32, float component33, float component34, float component41, float component42, float component43, float component44)
	{
		M11 = component11;
		M12 = component12;
		M13 = component13;
		M14 = component14;
		M21 = component21;
		M22 = component22;
		M23 = component23;
		M24 = component24;
		M31 = component31;
		M32 = component32;
		M33 = component33;
		M34 = component34;
		M41 = component41;
		M42 = component42;
		M43 = component43;
		M44 = component44;
	}

	public Matrix(float scalar)
	{
		M11 = scalar;
		M12 = 0f;
		M13 = 0f;
		M14 = 0f;
		M21 = 0f;
		M22 = scalar;
		M23 = 0f;
		M24 = 0f;
		M31 = 0f;
		M32 = 0f;
		M33 = scalar;
		M34 = 0f;
		M41 = 0f;
		M42 = 0f;
		M43 = 0f;
		M44 = scalar;
	}

	public static Matrix Invert(Matrix matrix)
	{
		float num = matrix.M33 * matrix.M44 - matrix.M43 * matrix.M34;
		float num2 = matrix.M23 * matrix.M44 - matrix.M43 * matrix.M24;
		float num3 = matrix.M23 * matrix.M34 - matrix.M33 * matrix.M24;
		float num4 = matrix.M13 * matrix.M44 - matrix.M43 * matrix.M14;
		float num5 = matrix.M13 * matrix.M34 - matrix.M33 * matrix.M14;
		float num6 = matrix.M13 * matrix.M24 - matrix.M23 * matrix.M14;
		Matrix result = new Matrix
		{
			M11 = matrix.M22 * num - matrix.M32 * num2 + matrix.M42 * num3,
			M12 = (0f - matrix.M12) * num + matrix.M32 * num4 - matrix.M42 * num5,
			M13 = matrix.M12 * num2 - matrix.M22 * num4 + matrix.M42 * num6,
			M14 = (0f - matrix.M12) * num3 + matrix.M22 * num5 - matrix.M32 * num6,
			M21 = (0f - matrix.M21) * num + matrix.M31 * num2 - matrix.M41 * num3,
			M22 = matrix.M11 * num - matrix.M31 * num4 + matrix.M41 * num5,
			M23 = (0f - matrix.M11) * num2 + matrix.M21 * num4 - matrix.M41 * num6,
			M24 = matrix.M11 * num3 - matrix.M21 * num5 + matrix.M31 * num6
		};
		num = matrix.M31 * matrix.M42 - matrix.M41 * matrix.M32;
		num2 = matrix.M21 * matrix.M42 - matrix.M41 * matrix.M22;
		num3 = matrix.M21 * matrix.M32 - matrix.M31 * matrix.M22;
		num4 = matrix.M11 * matrix.M42 - matrix.M41 * matrix.M12;
		num5 = matrix.M11 * matrix.M32 - matrix.M31 * matrix.M12;
		num6 = matrix.M11 * matrix.M22 - matrix.M21 * matrix.M12;
		result.M31 = matrix.M24 * num - matrix.M34 * num2 + matrix.M44 * num3;
		result.M32 = (0f - matrix.M14) * num + matrix.M34 * num4 - matrix.M44 * num5;
		result.M33 = matrix.M14 * num2 - matrix.M24 * num4 + matrix.M44 * num6;
		result.M34 = (0f - matrix.M14) * num3 + matrix.M24 * num5 - matrix.M34 * num6;
		result.M41 = (0f - matrix.M23) * num + matrix.M33 * num2 - matrix.M43 * num3;
		result.M42 = matrix.M13 * num - matrix.M33 * num4 + matrix.M43 * num5;
		result.M43 = (0f - matrix.M13) * num2 + matrix.M23 * num4 - matrix.M43 * num6;
		result.M44 = matrix.M13 * num3 - matrix.M23 * num5 + matrix.M33 * num6;
		num4 = matrix.M11 * result.M11 + matrix.M21 * result.M12 + matrix.M31 * result.M13 + matrix.M41 * result.M14;
		if (num4 > -1E-06f && num4 < 1E-06f)
		{
			throw new DivideByZeroException("Invert method failed, matrix is singular");
		}
		num4 = 1f / num4;
		result.M11 *= num4;
		result.M12 *= num4;
		result.M13 *= num4;
		result.M14 *= num4;
		result.M21 *= num4;
		result.M22 *= num4;
		result.M23 *= num4;
		result.M24 *= num4;
		result.M31 *= num4;
		result.M32 *= num4;
		result.M33 *= num4;
		result.M34 *= num4;
		result.M41 *= num4;
		result.M42 *= num4;
		result.M43 *= num4;
		result.M44 *= num4;
		return result;
	}

	public static Matrix CreateFromQuaternion(Quaternion quaternion)
	{
		Matrix result = default(Matrix);
		float num = quaternion.X * quaternion.X;
		float num2 = quaternion.X * quaternion.Y;
		float num3 = quaternion.X * quaternion.Z;
		float num4 = quaternion.X * quaternion.W;
		float num5 = quaternion.Y * quaternion.Y;
		float num6 = quaternion.Y * quaternion.Z;
		float num7 = quaternion.Y * quaternion.W;
		float num8 = quaternion.Z * quaternion.Z;
		float num9 = quaternion.Z * quaternion.W;
		result.M11 = 1f - 2f * (num5 + num8);
		result.M12 = 2f * (num2 + num9);
		result.M13 = 2f * (num3 - num7);
		result.M14 = 0f;
		result.M21 = 2f * (num2 - num9);
		result.M22 = 1f - 2f * (num + num8);
		result.M23 = 2f * (num6 + num4);
		result.M24 = 0f;
		result.M31 = 2f * (num3 + num7);
		result.M32 = 2f * (num6 - num4);
		result.M33 = 1f - 2f * (num + num5);
		result.M34 = 0f;
		result.M41 = 0f;
		result.M42 = 0f;
		result.M43 = 0f;
		result.M44 = 1f;
		return result;
	}

	public static Matrix CreateFromYawPitchRoll(float yaw, float pitch, float roll)
	{
		float num = (float)Math.Cos(yaw);
		float num2 = (float)Math.Sin(yaw);
		float num3 = (float)Math.Cos(pitch);
		float num4 = (float)Math.Sin(pitch);
		float num5 = (float)Math.Cos(roll);
		float num6 = (float)Math.Sin(roll);
		return new Matrix
		{
			M11 = num3 * num5,
			M12 = num2 * num4 * num5 - num * num6,
			M13 = num * num4 * num5 + num2 * num6,
			M14 = 0f,
			M21 = num3 * num6,
			M22 = num2 * num4 * num6 + num * num5,
			M23 = num * num4 * num6 - num2 * num5,
			M24 = 0f,
			M31 = 0f - num4,
			M32 = num2 * num3,
			M33 = num * num3,
			M34 = 0f,
			M41 = 0f,
			M42 = 0f,
			M43 = 0f,
			M44 = 1f
		};
	}

	public static Matrix Multiply(Matrix matrixA, Matrix matrixB)
	{
		return new Matrix
		{
			M11 = matrixA.M11 * matrixB.M11 + matrixA.M12 * matrixB.M21 + matrixA.M13 * matrixB.M31 + matrixA.M14 * matrixB.M41,
			M12 = matrixA.M11 * matrixB.M12 + matrixA.M12 * matrixB.M22 + matrixA.M13 * matrixB.M32 + matrixA.M14 * matrixB.M42,
			M13 = matrixA.M11 * matrixB.M13 + matrixA.M12 * matrixB.M23 + matrixA.M13 * matrixB.M33 + matrixA.M14 * matrixB.M43,
			M14 = matrixA.M11 * matrixB.M14 + matrixA.M12 * matrixB.M24 + matrixA.M13 * matrixB.M34 + matrixA.M14 * matrixB.M44,
			M21 = matrixA.M21 * matrixB.M11 + matrixA.M22 * matrixB.M21 + matrixA.M23 * matrixB.M31 + matrixA.M24 * matrixB.M41,
			M22 = matrixA.M21 * matrixB.M12 + matrixA.M22 * matrixB.M22 + matrixA.M23 * matrixB.M32 + matrixA.M24 * matrixB.M42,
			M23 = matrixA.M21 * matrixB.M13 + matrixA.M22 * matrixB.M23 + matrixA.M23 * matrixB.M33 + matrixA.M24 * matrixB.M43,
			M24 = matrixA.M21 * matrixB.M14 + matrixA.M22 * matrixB.M24 + matrixA.M23 * matrixB.M34 + matrixA.M24 * matrixB.M44,
			M31 = matrixA.M31 * matrixB.M11 + matrixA.M32 * matrixB.M21 + matrixA.M33 * matrixB.M31 + matrixA.M34 * matrixB.M41,
			M32 = matrixA.M31 * matrixB.M12 + matrixA.M32 * matrixB.M22 + matrixA.M33 * matrixB.M32 + matrixA.M34 * matrixB.M42,
			M33 = matrixA.M31 * matrixB.M13 + matrixA.M32 * matrixB.M23 + matrixA.M33 * matrixB.M33 + matrixA.M34 * matrixB.M43,
			M34 = matrixA.M31 * matrixB.M14 + matrixA.M32 * matrixB.M24 + matrixA.M33 * matrixB.M34 + matrixA.M34 * matrixB.M44,
			M41 = matrixA.M41 * matrixB.M11 + matrixA.M42 * matrixB.M21 + matrixA.M43 * matrixB.M31 + matrixA.M44 * matrixB.M41,
			M42 = matrixA.M41 * matrixB.M12 + matrixA.M42 * matrixB.M22 + matrixA.M43 * matrixB.M32 + matrixA.M44 * matrixB.M42,
			M43 = matrixA.M41 * matrixB.M13 + matrixA.M42 * matrixB.M23 + matrixA.M43 * matrixB.M33 + matrixA.M44 * matrixB.M43,
			M44 = matrixA.M41 * matrixB.M14 + matrixA.M42 * matrixB.M24 + matrixA.M43 * matrixB.M34 + matrixA.M44 * matrixB.M44
		};
	}
}
