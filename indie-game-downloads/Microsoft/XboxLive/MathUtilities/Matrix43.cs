using System;

namespace Microsoft.XboxLive.MathUtilities;

public struct Matrix43
{
	public float M11;

	public float M12;

	public float M13;

	public float M21;

	public float M22;

	public float M23;

	public float M31;

	public float M32;

	public float M33;

	public float M41;

	public float M42;

	public float M43;

	public void SetIdentity()
	{
		M11 = 1f;
		M12 = 0f;
		M13 = 0f;
		M21 = 0f;
		M22 = 1f;
		M23 = 0f;
		M31 = 0f;
		M32 = 0f;
		M33 = 1f;
		M41 = 0f;
		M42 = 0f;
		M43 = 0f;
	}

	public void ClearRotation()
	{
		M11 = 1f;
		M12 = 0f;
		M13 = 0f;
		M21 = 0f;
		M22 = 1f;
		M23 = 0f;
		M31 = 0f;
		M32 = 0f;
		M33 = 1f;
	}

	public Matrix43 InvertEuclidean()
	{
		Matrix43 result = default(Matrix43);
		result.M11 = M11;
		result.M21 = M12;
		result.M31 = M13;
		result.M12 = M21;
		result.M22 = M22;
		result.M32 = M23;
		result.M13 = M31;
		result.M23 = M32;
		result.M33 = M33;
		result.M41 = 0f - (M41 * result.M11 + M42 * result.M21 + M43 * result.M31);
		result.M42 = 0f - (M41 * result.M12 + M42 * result.M22 + M43 * result.M32);
		result.M43 = 0f - (M41 * result.M13 + M42 * result.M23 + M43 * result.M33);
		return result;
	}

	public Matrix43 Invert()
	{
		float num = M11 * (M33 * M22 - M32 * M23) - M21 * (M33 * M12 - M32 * M13) + M31 * (M23 * M12 - M22 * M13);
		if (num > -1E-06f && num < 1E-06f)
		{
			throw new DivideByZeroException("Invert method failed, matrix is singular");
		}
		num = 1f / num;
		Matrix43 result = default(Matrix43);
		result.M11 = num * (M33 * M22 - M32 * M23);
		result.M12 = num * (M13 * M32 - M12 * M33);
		result.M13 = num * (M23 * M12 - M22 * M13);
		result.M21 = num * (M31 * M23 - M33 * M21);
		result.M22 = num * (M11 * M33 - M13 * M31);
		result.M23 = num * (M21 * M13 - M23 * M11);
		result.M31 = num * (M32 * M21 - M31 * M22);
		result.M32 = num * (M12 * M31 - M11 * M32);
		result.M33 = num * (M22 * M11 - M21 * M12);
		result.M41 = 0f - (result.M11 * M41 + result.M21 * M42 + result.M31 * M43);
		result.M42 = 0f - (result.M12 * M41 + result.M22 * M42 + result.M32 * M43);
		result.M43 = 0f - (result.M13 * M41 + result.M23 * M42 + result.M33 * M43);
		return result;
	}

	public void Scale(ref Vector3 scaleFactor)
	{
		M11 *= scaleFactor.X;
		M12 *= scaleFactor.Y;
		M13 *= scaleFactor.Z;
		M21 *= scaleFactor.X;
		M22 *= scaleFactor.Y;
		M23 *= scaleFactor.Z;
		M31 *= scaleFactor.X;
		M32 *= scaleFactor.Y;
		M33 *= scaleFactor.Z;
	}

	public void SetTranslation(float translationX, float translationY, float translationZ)
	{
		M41 = translationX;
		M42 = translationY;
		M43 = translationZ;
	}

	public void SetTranslation(Vector3 translation)
	{
		M41 = translation.X;
		M42 = translation.Y;
		M43 = translation.Z;
	}

	public Vector3 GetTranslation()
	{
		return new Vector3
		{
			X = M41,
			Y = M42,
			Z = M43
		};
	}

	public Quaternion GetRotationQuaternion()
	{
		Quaternion result = default(Quaternion);
		float num = M11 + M22 + M33 + 1f;
		if (num > 0.01f)
		{
			result.W = num;
			result.X = M23 - M32;
			result.Y = M31 - M13;
			result.Z = M12 - M21;
		}
		else if (M11 > M22 && M11 > M33)
		{
			result.W = M32 - M23;
			result.X = M33 + M22 - M11 - 1f;
			result.Y = 0f - (M21 + M12);
			result.Z = 0f - (M31 + M13);
		}
		else if (M22 > M33)
		{
			result.W = M31 - M13;
			result.X = M21 + M12;
			result.Y = 1f - M11 + M22 - M33;
			result.Z = M32 + M23;
		}
		else
		{
			result.W = M21 - M12;
			result.X = 0f - (M31 + M13);
			result.Y = 0f - (M32 + M23);
			result.Z = M11 + M22 - M33 - 1f;
		}
		float num2 = 1f / (float)Math.Sqrt(result.X * result.X + result.Y * result.Y + result.Z * result.Z + result.W * result.W);
		result.X *= num2;
		result.Y *= num2;
		result.Z *= num2;
		result.W *= num2;
		return result;
	}

	public void SetRotationEuler(float angleX, float angleY, float angleZ)
	{
		float num = (float)Math.Cos(angleX);
		float num2 = (float)Math.Sin(angleX);
		float num3 = (float)Math.Cos(angleY);
		float num4 = (float)Math.Sin(angleY);
		float num5 = (float)Math.Cos(angleZ);
		float num6 = (float)Math.Sin(angleZ);
		M11 = num3 * num5;
		M12 = num2 * num4 * num5 - num * num6;
		M13 = num * num4 * num5 + num2 * num6;
		M21 = num3 * num6;
		M22 = num2 * num4 * num6 + num * num5;
		M23 = num * num4 * num6 - num2 * num5;
		M31 = 0f - num4;
		M32 = num2 * num3;
		M33 = num * num3;
	}

	public static Matrix43 CreateFromQuaternion(Quaternion quaternion)
	{
		Matrix43 result = default(Matrix43);
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
		result.M21 = 2f * (num2 - num9);
		result.M22 = 1f - 2f * (num + num8);
		result.M23 = 2f * (num6 + num4);
		result.M31 = 2f * (num3 + num7);
		result.M32 = 2f * (num6 - num4);
		result.M33 = 1f - 2f * (num + num5);
		result.M41 = 0f;
		result.M42 = 0f;
		result.M43 = 0f;
		return result;
	}
}
