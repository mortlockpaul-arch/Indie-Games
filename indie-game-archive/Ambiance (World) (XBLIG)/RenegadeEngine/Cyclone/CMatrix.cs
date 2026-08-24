using Microsoft.Xna.Framework;

namespace RenegadeEngine.Cyclone;

public static class CMatrix
{
	public static void Get3x3Transpose(ref Matrix matrix, out Matrix result)
	{
		result = Matrix.Identity;
		result.M11 = matrix.M11;
		result.M12 = matrix.M21;
		result.M13 = matrix.M31;
		result.M21 = matrix.M12;
		result.M22 = matrix.M22;
		result.M23 = matrix.M32;
		result.M31 = matrix.M13;
		result.M32 = matrix.M23;
		result.M33 = matrix.M33;
	}

	public static void SetBlockInertiaTensor(Vector3 halfSizes, float mass, out Matrix result)
	{
		result = default(Matrix);
		Vector3 vector = Vector3.Cross(halfSizes, halfSizes);
		SetInertiaTensorCoeffs(PhysicsDefinitions.SleepEpsilon * mass * (vector.Y + vector.Z), PhysicsDefinitions.SleepEpsilon * mass * (vector.X + vector.Z), PhysicsDefinitions.SleepEpsilon * mass * (vector.X + vector.Y), out result);
	}

	public static void SetInertiaTensorCoeffs(float x, float y, float z, out Matrix result)
	{
		result = default(Matrix);
		result.M11 = x;
		result.M12 = 0f;
		result.M13 = 0f;
		result.M14 = 0f;
		result.M21 = 0f;
		result.M22 = y;
		result.M23 = 0f;
		result.M24 = 0f;
		result.M31 = 0f;
		result.M32 = 0f;
		result.M33 = z;
		result.M34 = 0f;
		result.M41 = 0f;
		result.M42 = 0f;
		result.M43 = 0f;
		result.M44 = 1f;
	}

	public static void SetSkewSymmetric(ref Vector3 vector, out Matrix skewSymmetricMatrix)
	{
		skewSymmetricMatrix = Matrix.Identity;
		skewSymmetricMatrix.M11 = 0f;
		skewSymmetricMatrix.M12 = 0f - vector.Z;
		skewSymmetricMatrix.M13 = vector.Y;
		skewSymmetricMatrix.M21 = vector.X;
		skewSymmetricMatrix.M22 = 0f;
		skewSymmetricMatrix.M23 = 0f - vector.X;
		skewSymmetricMatrix.M31 = 0f - vector.Y;
		skewSymmetricMatrix.M32 = vector.X;
		skewSymmetricMatrix.M33 = 0f;
	}

	public static void Transform3x3(ref Vector3 result, ref Matrix transform)
	{
		result.X = result.X * transform.M11 + result.Y * transform.M12 + result.Z * transform.M13;
		result.Y = result.X * transform.M21 + result.Y * transform.M22 + result.Z * transform.M23;
		result.Z = result.X * transform.M31 + result.Y * transform.M32 + result.Z * transform.M33;
	}

	public static void Transform3x3Transpose(ref Vector3 result, ref Matrix transform)
	{
		result.X = result.X * transform.M11 + result.Y * transform.M21 + result.Z * transform.M31;
		result.Y = result.X * transform.M12 + result.Y * transform.M22 + result.Z * transform.M32;
		result.Z = result.X * transform.M13 + result.Y * transform.M23 + result.Z * transform.M33;
	}

	public static void Invert(ref Matrix matrix)
	{
		float num = matrix.M11 * matrix.M22;
		float num2 = matrix.M11 * matrix.M23;
		float num3 = matrix.M12 * matrix.M21;
		float num4 = matrix.M13 * matrix.M21;
		float num5 = matrix.M12 * matrix.M31;
		float num6 = matrix.M13 * matrix.M31;
		float num7 = num * matrix.M33 - num2 * matrix.M32 - num3 * matrix.M33 + num4 * matrix.M32 + num5 * matrix.M23 - num6 * matrix.M22;
		if (num7 != 0f)
		{
			float num8 = 1f / num7;
			matrix.M11 = (matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32) * num8;
			matrix.M12 = (0f - (matrix.M12 * matrix.M33 - matrix.M13 * matrix.M32)) * num8;
			matrix.M13 = (matrix.M12 * matrix.M23 - matrix.M13 * matrix.M22) * num8;
			matrix.M21 = (0f - (matrix.M21 * matrix.M33 - matrix.M23 * matrix.M31)) * num8;
			matrix.M22 = (matrix.M11 * matrix.M33 - num6) * num8;
			matrix.M23 = (0f - (num2 - num4)) * num8;
			matrix.M31 = (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31) * num8;
			matrix.M32 = (0f - (matrix.M11 * matrix.M32 - num5)) * num8;
			matrix.M33 = (num - num3) * num8;
		}
	}

	public static Matrix Inverse(ref Matrix matrix)
	{
		Matrix matrix2 = matrix;
		Invert(ref matrix2);
		return matrix2;
	}
}
