using System;

namespace Microsoft.XboxLive.MathUtilities;

public static class MatrixMath
{
	public static Matrix43 Multiply(ref Matrix43 matrixA, ref Matrix43 matrixB)
	{
		return new Matrix43
		{
			M11 = matrixA.M11 * matrixB.M11 + matrixA.M12 * matrixB.M21 + matrixA.M13 * matrixB.M31,
			M21 = matrixA.M21 * matrixB.M11 + matrixA.M22 * matrixB.M21 + matrixA.M23 * matrixB.M31,
			M31 = matrixA.M31 * matrixB.M11 + matrixA.M32 * matrixB.M21 + matrixA.M33 * matrixB.M31,
			M41 = matrixA.M41 * matrixB.M11 + matrixA.M42 * matrixB.M21 + matrixA.M43 * matrixB.M31 + matrixB.M41,
			M12 = matrixA.M11 * matrixB.M12 + matrixA.M12 * matrixB.M22 + matrixA.M13 * matrixB.M32,
			M22 = matrixA.M21 * matrixB.M12 + matrixA.M22 * matrixB.M22 + matrixA.M23 * matrixB.M32,
			M32 = matrixA.M31 * matrixB.M12 + matrixA.M32 * matrixB.M22 + matrixA.M33 * matrixB.M32,
			M42 = matrixA.M41 * matrixB.M12 + matrixA.M42 * matrixB.M22 + matrixA.M43 * matrixB.M32 + matrixB.M42,
			M13 = matrixA.M11 * matrixB.M13 + matrixA.M12 * matrixB.M23 + matrixA.M13 * matrixB.M33,
			M23 = matrixA.M21 * matrixB.M13 + matrixA.M22 * matrixB.M23 + matrixA.M23 * matrixB.M33,
			M33 = matrixA.M31 * matrixB.M13 + matrixA.M32 * matrixB.M23 + matrixA.M33 * matrixB.M33,
			M43 = matrixA.M41 * matrixB.M13 + matrixA.M42 * matrixB.M23 + matrixA.M43 * matrixB.M33 + matrixB.M43
		};
	}

	public static Matrix Multiply(ref Matrix43 matrixA, ref Matrix matrixB)
	{
		return new Matrix
		{
			M11 = matrixA.M11 * matrixB.M11 + matrixA.M12 * matrixB.M21 + matrixA.M13 * matrixB.M31,
			M12 = matrixA.M11 * matrixB.M12 + matrixA.M12 * matrixB.M22 + matrixA.M13 * matrixB.M32,
			M13 = matrixA.M11 * matrixB.M13 + matrixA.M12 * matrixB.M23 + matrixA.M13 * matrixB.M33,
			M14 = matrixA.M11 * matrixB.M14 + matrixA.M12 * matrixB.M24 + matrixA.M13 * matrixB.M34,
			M21 = matrixA.M21 * matrixB.M11 + matrixA.M22 * matrixB.M21 + matrixA.M23 * matrixB.M31,
			M22 = matrixA.M21 * matrixB.M12 + matrixA.M22 * matrixB.M22 + matrixA.M23 * matrixB.M32,
			M23 = matrixA.M21 * matrixB.M13 + matrixA.M22 * matrixB.M23 + matrixA.M23 * matrixB.M33,
			M24 = matrixA.M21 * matrixB.M14 + matrixA.M22 * matrixB.M24 + matrixA.M23 * matrixB.M34,
			M31 = matrixA.M31 * matrixB.M11 + matrixA.M32 * matrixB.M21 + matrixA.M33 * matrixB.M31,
			M32 = matrixA.M31 * matrixB.M12 + matrixA.M32 * matrixB.M22 + matrixA.M33 * matrixB.M32,
			M33 = matrixA.M31 * matrixB.M13 + matrixA.M32 * matrixB.M23 + matrixA.M33 * matrixB.M33,
			M34 = matrixA.M31 * matrixB.M14 + matrixA.M32 * matrixB.M24 + matrixA.M33 * matrixB.M34,
			M41 = matrixA.M41 * matrixB.M11 + matrixA.M42 * matrixB.M21 + matrixA.M43 * matrixB.M31 + matrixB.M41,
			M42 = matrixA.M41 * matrixB.M12 + matrixA.M42 * matrixB.M22 + matrixA.M43 * matrixB.M32 + matrixB.M42,
			M43 = matrixA.M41 * matrixB.M13 + matrixA.M42 * matrixB.M23 + matrixA.M43 * matrixB.M33 + matrixB.M43,
			M44 = matrixA.M41 * matrixB.M14 + matrixA.M42 * matrixB.M24 + matrixA.M43 * matrixB.M34 + matrixB.M44
		};
	}

	public static Matrix43 InvertEuclidean(Matrix43 matrix)
	{
		Matrix43 result = default(Matrix43);
		result.M11 = matrix.M11;
		result.M21 = matrix.M12;
		result.M31 = matrix.M13;
		result.M12 = matrix.M21;
		result.M22 = matrix.M22;
		result.M32 = matrix.M23;
		result.M13 = matrix.M31;
		result.M23 = matrix.M32;
		result.M33 = matrix.M33;
		result.M41 = 0f - (matrix.M41 * result.M11 + matrix.M42 * result.M21 + matrix.M43 * result.M31);
		result.M42 = 0f - (matrix.M41 * result.M12 + matrix.M42 * result.M22 + matrix.M43 * result.M32);
		result.M43 = 0f - (matrix.M41 * result.M13 + matrix.M42 * result.M23 + matrix.M43 * result.M33);
		return result;
	}

	public static void SetTranslation(ref Matrix matrix, float translationX, float translationY, float translationZ)
	{
		matrix.M41 = translationX;
		matrix.M42 = translationY;
		matrix.M43 = translationZ;
	}

	public static Vector3 GetRotationEuler(Matrix matrix)
	{
		Vector3 result = default(Vector3);
		if (matrix.M31 < 1f && matrix.M31 > -1f)
		{
			result.X = (float)Math.Atan2(matrix.M32, matrix.M33);
			result.Z = (float)Math.Atan2(matrix.M21, matrix.M11);
			result.Y = (float)Math.Asin(0f - matrix.M31);
		}
		else
		{
			result.Z = 0f;
			if (matrix.M31 < 0f)
			{
				result.Y = (float)Math.PI / 2f;
				result.X = (float)Math.Atan2(matrix.M12, matrix.M13);
			}
			else
			{
				result.Y = -(float)Math.PI / 2f;
				result.X = (float)Math.Atan2(0f - matrix.M12, 0f - matrix.M13);
			}
		}
		return result;
	}

	public static void SetFocalLength(ref Matrix matrix, float focalLength)
	{
		matrix.M14 = matrix.M13 * focalLength;
		matrix.M24 = matrix.M23 * focalLength;
		matrix.M34 = matrix.M33 * focalLength;
		matrix.M44 = matrix.M43 * focalLength;
	}

	public static float GetProjectionFocalLength(Matrix matrix)
	{
		return matrix.M34 / matrix.M33;
	}

	public static Matrix CreateMatrixFromEulerOffset(float angleX, float angleY, float angleZ, float positionX, float positionY, float positionZ, float focalLength)
	{
		Matrix matrix = Matrix.CreateFromYawPitchRoll(angleX, angleY, angleZ);
		matrix.M41 = positionX;
		matrix.M42 = positionY;
		matrix.M43 = positionZ;
		SetFocalLength(ref matrix, focalLength);
		return matrix;
	}

	public static Matrix CreateMatrixFromEulerOffset(Vector3 eulers, Vector3 translation, float focal)
	{
		return CreateMatrixFromEulerOffset(eulers.X, eulers.Y, eulers.Z, translation.X, translation.Y, translation.Z, focal);
	}

	public static Matrix InvertEuclidean(Matrix matrix)
	{
		Matrix result = default(Matrix);
		result.M11 = matrix.M11;
		result.M21 = matrix.M12;
		result.M31 = matrix.M13;
		result.M12 = matrix.M21;
		result.M22 = matrix.M22;
		result.M32 = matrix.M23;
		result.M13 = matrix.M31;
		result.M23 = matrix.M32;
		result.M33 = matrix.M33;
		result.M14 = 0f;
		result.M24 = 0f;
		result.M34 = 0f;
		result.M44 = 1f;
		result.M41 = 0f - (matrix.M41 * result.M11 + matrix.M42 * result.M21 + matrix.M43 * result.M31);
		result.M42 = 0f - (matrix.M41 * result.M12 + matrix.M42 * result.M22 + matrix.M43 * result.M32);
		result.M43 = 0f - (matrix.M41 * result.M13 + matrix.M42 * result.M23 + matrix.M43 * result.M33);
		return result;
	}
}
