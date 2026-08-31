using System;
using Microsoft.Xna.Framework;

namespace BEPUphysics.MathExtensions;

/// <summary>
/// 3 row, 3 column matrix.
/// </summary>
public struct Matrix3X3
{
	/// <summary>
	/// Value at row 1, column 1 of the matrix.
	/// </summary>
	public float M11;

	/// <summary>
	/// Value at row 1, column 2 of the matrix.
	/// </summary>
	public float M12;

	/// <summary>
	/// Value at row 1, column 3 of the matrix.
	/// </summary>
	public float M13;

	/// <summary>
	/// Value at row 2, column 1 of the matrix.
	/// </summary>
	public float M21;

	/// <summary>
	/// Value at row 2, column 2 of the matrix.
	/// </summary>
	public float M22;

	/// <summary>
	/// Value at row 2, column 3 of the matrix.
	/// </summary>
	public float M23;

	/// <summary>
	/// Value at row 3, column 1 of the matrix.
	/// </summary>
	public float M31;

	/// <summary>
	/// Value at row 3, column 2 of the matrix.
	/// </summary>
	public float M32;

	/// <summary>
	/// Value at row 3, column 3 of the matrix.
	/// </summary>
	public float M33;

	/// <summary>
	/// Gets the 3x3 identity matrix.
	/// </summary>
	public static Matrix3X3 Identity => new Matrix3X3(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

	/// <summary>
	/// Gets or sets the backward vector of the matrix.
	/// </summary>
	public Vector3 Backward
	{
		get
		{
			return new Vector3
			{
				X = M31,
				Y = M32,
				Z = M33
			};
		}
		set
		{
			M31 = value.X;
			M32 = value.Y;
			M33 = value.Z;
		}
	}

	/// <summary>
	/// Gets or sets the down vector of the matrix.
	/// </summary>
	public Vector3 Down
	{
		get
		{
			return new Vector3
			{
				X = 0f - M21,
				Y = 0f - M22,
				Z = 0f - M23
			};
		}
		set
		{
			M21 = 0f - value.X;
			M22 = 0f - value.Y;
			M23 = 0f - value.Z;
		}
	}

	/// <summary>
	/// Gets or sets the forward vector of the matrix.
	/// </summary>
	public Vector3 Forward
	{
		get
		{
			return new Vector3
			{
				X = 0f - M31,
				Y = 0f - M32,
				Z = 0f - M33
			};
		}
		set
		{
			M31 = 0f - value.X;
			M32 = 0f - value.Y;
			M33 = 0f - value.Z;
		}
	}

	/// <summary>
	/// Gets or sets the left vector of the matrix.
	/// </summary>
	public Vector3 Left
	{
		get
		{
			return new Vector3
			{
				X = 0f - M11,
				Y = 0f - M12,
				Z = 0f - M13
			};
		}
		set
		{
			M11 = 0f - value.X;
			M12 = 0f - value.Y;
			M13 = 0f - value.Z;
		}
	}

	/// <summary>
	/// Gets or sets the right vector of the matrix.
	/// </summary>
	public Vector3 Right
	{
		get
		{
			return new Vector3
			{
				X = M11,
				Y = M12,
				Z = M13
			};
		}
		set
		{
			M11 = value.X;
			M12 = value.Y;
			M13 = value.Z;
		}
	}

	/// <summary>
	/// Gets or sets the up vector of the matrix.
	/// </summary>
	public Vector3 Up
	{
		get
		{
			return new Vector3
			{
				X = M21,
				Y = M22,
				Z = M23
			};
		}
		set
		{
			M21 = value.X;
			M22 = value.Y;
			M23 = value.Z;
		}
	}

	/// <summary>
	/// Constructs a new 3 row, 3 column matrix.
	/// </summary>
	/// <param name="m11">Value at row 1, column 1 of the matrix.</param>
	/// <param name="m12">Value at row 1, column 2 of the matrix.</param>
	/// <param name="m13">Value at row 1, column 3 of the matrix.</param>
	/// <param name="m21">Value at row 2, column 1 of the matrix.</param>
	/// <param name="m22">Value at row 2, column 2 of the matrix.</param>
	/// <param name="m23">Value at row 2, column 3 of the matrix.</param>
	/// <param name="m31">Value at row 3, column 1 of the matrix.</param>
	/// <param name="m32">Value at row 3, column 2 of the matrix.</param>
	/// <param name="m33">Value at row 3, column 3 of the matrix.</param>
	public Matrix3X3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
	{
		M11 = m11;
		M12 = m12;
		M13 = m13;
		M21 = m21;
		M22 = m22;
		M23 = m23;
		M31 = m31;
		M32 = m32;
		M33 = m33;
	}

	/// <summary>
	/// Adds the two matrices together on a per-element basis.
	/// </summary>
	/// <param name="a">First matrix to add.</param>
	/// <param name="b">Second matrix to add.</param>
	/// <param name="result">Sum of the two matrices.</param>
	public static void Add(ref Matrix3X3 a, ref Matrix3X3 b, out Matrix3X3 result)
	{
		float m = a.M11 + b.M11;
		float m2 = a.M12 + b.M12;
		float m3 = a.M13 + b.M13;
		float m4 = a.M21 + b.M21;
		float m5 = a.M22 + b.M22;
		float m6 = a.M23 + b.M23;
		float m7 = a.M31 + b.M31;
		float m8 = a.M32 + b.M32;
		float m9 = a.M33 + b.M33;
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
	}

	/// <summary>
	/// Adds the two matrices together on a per-element basis.
	/// </summary>
	/// <param name="a">First matrix to add.</param>
	/// <param name="b">Second matrix to add.</param>
	/// <param name="result">Sum of the two matrices.</param>
	public static void Add(ref Matrix a, ref Matrix3X3 b, out Matrix3X3 result)
	{
		float m = a.M11 + b.M11;
		float m2 = a.M12 + b.M12;
		float m3 = a.M13 + b.M13;
		float m4 = a.M21 + b.M21;
		float m5 = a.M22 + b.M22;
		float m6 = a.M23 + b.M23;
		float m7 = a.M31 + b.M31;
		float m8 = a.M32 + b.M32;
		float m9 = a.M33 + b.M33;
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
	}

	/// <summary>
	/// Adds the two matrices together on a per-element basis.
	/// </summary>
	/// <param name="a">First matrix to add.</param>
	/// <param name="b">Second matrix to add.</param>
	/// <param name="result">Sum of the two matrices.</param>
	public static void Add(ref Matrix3X3 a, ref Matrix b, out Matrix3X3 result)
	{
		float m = a.M11 + b.M11;
		float m2 = a.M12 + b.M12;
		float m3 = a.M13 + b.M13;
		float m4 = a.M21 + b.M21;
		float m5 = a.M22 + b.M22;
		float m6 = a.M23 + b.M23;
		float m7 = a.M31 + b.M31;
		float m8 = a.M32 + b.M32;
		float m9 = a.M33 + b.M33;
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
	}

	/// <summary>
	/// Adds the two matrices together on a per-element basis.
	/// </summary>
	/// <param name="a">First matrix to add.</param>
	/// <param name="b">Second matrix to add.</param>
	/// <param name="result">Sum of the two matrices.</param>
	public static void Add(ref Matrix a, ref Matrix b, out Matrix3X3 result)
	{
		float m = a.M11 + b.M11;
		float m2 = a.M12 + b.M12;
		float m3 = a.M13 + b.M13;
		float m4 = a.M21 + b.M21;
		float m5 = a.M22 + b.M22;
		float m6 = a.M23 + b.M23;
		float m7 = a.M31 + b.M31;
		float m8 = a.M32 + b.M32;
		float m9 = a.M33 + b.M33;
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
	}

	/// <summary>
	/// Creates a skew symmetric matrix M from vector A such that M * B for some other vector B is equivalent to the cross product of A and B.
	/// </summary>
	/// <param name="v">Vector to base the matrix on.</param>
	/// <param name="result">Skew-symmetric matrix result.</param>
	public static void CreateCrossProduct(ref Vector3 v, out Matrix3X3 result)
	{
		result.M11 = 0f;
		result.M12 = 0f - v.Z;
		result.M13 = v.Y;
		result.M21 = v.Z;
		result.M22 = 0f;
		result.M23 = 0f - v.X;
		result.M31 = 0f - v.Y;
		result.M32 = v.X;
		result.M33 = 0f;
	}

	/// <summary>
	/// Creates a 3x3 matrix from an XNA 4x4 matrix.
	/// </summary>
	/// <param name="matrix4X4">Matrix to extract a 3x3 matrix from.</param>
	/// <param name="matrix3X3">Upper 3x3 matrix extracted from the XNA matrix.</param>
	public static void CreateFromMatrix(ref Matrix matrix4X4, out Matrix3X3 matrix3X3)
	{
		matrix3X3.M11 = matrix4X4.M11;
		matrix3X3.M12 = matrix4X4.M12;
		matrix3X3.M13 = matrix4X4.M13;
		matrix3X3.M21 = matrix4X4.M21;
		matrix3X3.M22 = matrix4X4.M22;
		matrix3X3.M23 = matrix4X4.M23;
		matrix3X3.M31 = matrix4X4.M31;
		matrix3X3.M32 = matrix4X4.M32;
		matrix3X3.M33 = matrix4X4.M33;
	}

	/// <summary>
	/// Creates a 3x3 matrix from an XNA 4x4 matrix.
	/// </summary>
	/// <param name="matrix4X4">Matrix to extract a 3x3 matrix from.</param>
	/// <returns>Upper 3x3 matrix extracted from the XNA matrix.</returns>
	public static Matrix3X3 CreateFromMatrix(Matrix matrix4X4)
	{
		Matrix3X3 result = default(Matrix3X3);
		result.M11 = matrix4X4.M11;
		result.M12 = matrix4X4.M12;
		result.M13 = matrix4X4.M13;
		result.M21 = matrix4X4.M21;
		result.M22 = matrix4X4.M22;
		result.M23 = matrix4X4.M23;
		result.M31 = matrix4X4.M31;
		result.M32 = matrix4X4.M32;
		result.M33 = matrix4X4.M33;
		return result;
	}

	/// <summary>
	/// Constructs a uniform scaling matrix.
	/// </summary>
	/// <param name="scale">Value to use in the diagonal.</param>
	/// <param name="matrix">Scaling matrix.</param>
	public static void CreateScale(float scale, out Matrix3X3 matrix)
	{
		matrix = new Matrix3X3
		{
			M11 = scale,
			M22 = scale,
			M33 = scale
		};
	}

	/// <summary>
	/// Constructs a uniform scaling matrix.
	/// </summary>
	/// <param name="scale">Value to use in the diagonal.</param>
	/// <returns>Scaling matrix.</returns>
	public static Matrix3X3 CreateScale(float scale)
	{
		return new Matrix3X3
		{
			M11 = scale,
			M22 = scale,
			M33 = scale
		};
	}

	/// <summary>
	/// Constructs a non-uniform scaling matrix.
	/// </summary>
	/// <param name="scale">Values defining the axis scales.</param>
	/// <param name="matrix">Scaling matrix.</param>
	public static void CreateScale(ref Vector3 scale, out Matrix3X3 matrix)
	{
		matrix = new Matrix3X3
		{
			M11 = scale.X,
			M22 = scale.Y,
			M33 = scale.Z
		};
	}

	/// <summary>
	/// Constructs a non-uniform scaling matrix.
	/// </summary>
	/// <param name="scale">Values defining the axis scales.</param>
	/// <returns>Scaling matrix.</returns>
	public static Matrix3X3 CreateScale(ref Vector3 scale)
	{
		return new Matrix3X3
		{
			M11 = scale.X,
			M22 = scale.Y,
			M33 = scale.Z
		};
	}

	/// <summary>
	/// Constructs a non-uniform scaling matrix.
	/// </summary>
	/// <param name="x">Scaling along the x axis.</param>
	/// <param name="y">Scaling along the y axis.</param>
	/// <param name="z">Scaling along the z axis.</param>
	/// <param name="matrix">Scaling matrix.</param>
	public static void CreateScale(float x, float y, float z, out Matrix3X3 matrix)
	{
		matrix = new Matrix3X3
		{
			M11 = x,
			M22 = y,
			M33 = z
		};
	}

	/// <summary>
	/// Constructs a non-uniform scaling matrix.
	/// </summary>
	/// <param name="x">Scaling along the x axis.</param>
	/// <param name="y">Scaling along the y axis.</param>
	/// <param name="z">Scaling along the z axis.</param>
	/// <returns>Scaling matrix.</returns>
	public static Matrix3X3 CreateScale(float x, float y, float z)
	{
		return new Matrix3X3
		{
			M11 = x,
			M22 = y,
			M33 = z
		};
	}

	/// <summary>
	/// Inverts the given matix.
	/// </summary>
	/// <param name="matrix">Matrix to be inverted.</param>
	/// <param name="result">Inverted matrix.</param>
	public static void Invert(ref Matrix3X3 matrix, out Matrix3X3 result)
	{
		float num = 1f / matrix.Determinant();
		float m = (matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32) * num;
		float m2 = (matrix.M13 * matrix.M32 - matrix.M33 * matrix.M12) * num;
		float m3 = (matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13) * num;
		float m4 = (matrix.M23 * matrix.M31 - matrix.M21 * matrix.M33) * num;
		float m5 = (matrix.M11 * matrix.M33 - matrix.M13 * matrix.M31) * num;
		float m6 = (matrix.M13 * matrix.M21 - matrix.M11 * matrix.M23) * num;
		float m7 = (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31) * num;
		float m8 = (matrix.M12 * matrix.M31 - matrix.M11 * matrix.M32) * num;
		float m9 = (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21) * num;
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
	}

	/// <summary>
	/// Inverts the largest nonsingular submatrix in the matrix, excluding 2x2's that involve M13 or M31, and excluding 1x1's that include nondiagonal elements.
	/// </summary>
	/// <param name="matrix">Matrix to be inverted.</param>
	/// <param name="result">Inverted matrix.</param>
	internal static void AdaptiveInvert(ref Matrix3X3 matrix, out Matrix3X3 result)
	{
		float num = 1f / matrix.AdaptiveDeterminant(out var subMatrixCode);
		float m;
		float m2;
		float m3;
		float m4;
		float m5;
		float m6;
		float m7;
		float m8;
		float m9;
		switch (subMatrixCode)
		{
		case 0:
			m = (matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32) * num;
			m2 = (matrix.M13 * matrix.M32 - matrix.M33 * matrix.M12) * num;
			m3 = (matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13) * num;
			m4 = (matrix.M23 * matrix.M31 - matrix.M21 * matrix.M33) * num;
			m5 = (matrix.M11 * matrix.M33 - matrix.M13 * matrix.M31) * num;
			m6 = (matrix.M13 * matrix.M21 - matrix.M11 * matrix.M23) * num;
			m7 = (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31) * num;
			m8 = (matrix.M12 * matrix.M31 - matrix.M11 * matrix.M32) * num;
			m9 = (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21) * num;
			break;
		case 1:
			m = matrix.M22 * num;
			m2 = (0f - matrix.M12) * num;
			m3 = 0f;
			m4 = (0f - matrix.M21) * num;
			m5 = matrix.M11 * num;
			m6 = 0f;
			m7 = 0f;
			m8 = 0f;
			m9 = 0f;
			break;
		case 2:
			m = 0f;
			m2 = 0f;
			m3 = 0f;
			m4 = 0f;
			m5 = matrix.M33 * num;
			m6 = (0f - matrix.M23) * num;
			m7 = 0f;
			m8 = (0f - matrix.M32) * num;
			m9 = matrix.M22 * num;
			break;
		case 3:
			m = matrix.M33 * num;
			m2 = 0f;
			m3 = (0f - matrix.M13) * num;
			m4 = 0f;
			m5 = 0f;
			m6 = 0f;
			m7 = (0f - matrix.M31) * num;
			m8 = 0f;
			m9 = matrix.M11 * num;
			break;
		case 4:
			m = 1f / matrix.M11;
			m2 = 0f;
			m3 = 0f;
			m4 = 0f;
			m5 = 0f;
			m6 = 0f;
			m7 = 0f;
			m8 = 0f;
			m9 = 0f;
			break;
		case 5:
			m = 0f;
			m2 = 0f;
			m3 = 0f;
			m4 = 0f;
			m5 = 1f / matrix.M22;
			m6 = 0f;
			m7 = 0f;
			m8 = 0f;
			m9 = 0f;
			break;
		case 6:
			m = 0f;
			m2 = 0f;
			m3 = 0f;
			m4 = 0f;
			m5 = 0f;
			m6 = 0f;
			m7 = 0f;
			m8 = 0f;
			m9 = 1f / matrix.M33;
			break;
		default:
			m = 0f;
			m2 = 0f;
			m3 = 0f;
			m4 = 0f;
			m5 = 0f;
			m6 = 0f;
			m7 = 0f;
			m8 = 0f;
			m9 = 0f;
			break;
		}
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
	}

	/// <summary>
	/// Multiplies the two matrices.
	/// </summary>
	/// <param name="a">First matrix to multiply.</param>
	/// <param name="b">Second matrix to multiply.</param>
	/// <returns>Product of the multiplication.</returns>
	public static Matrix3X3 operator *(Matrix3X3 a, Matrix3X3 b)
	{
		float m = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
		float m2 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
		float m3 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;
		float m4 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
		float m5 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
		float m6 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;
		float m7 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
		float m8 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
		float m9 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;
		Matrix3X3 result = default(Matrix3X3);
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
		return result;
	}

	/// <summary>
	/// Multiplies the two matrices.
	/// </summary>
	/// <param name="a">First matrix to multiply.</param>
	/// <param name="b">Second matrix to multiply.</param>
	/// <param name="result">Product of the multiplication.</param>
	public static void Multiply(ref Matrix3X3 a, ref Matrix3X3 b, out Matrix3X3 result)
	{
		float m = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
		float m2 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
		float m3 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;
		float m4 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
		float m5 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
		float m6 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;
		float m7 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
		float m8 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
		float m9 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
	}

	/// <summary>
	/// Multiplies the two matrices.
	/// </summary>
	/// <param name="a">First matrix to multiply.</param>
	/// <param name="b">Second matrix to multiply.</param>
	/// <param name="result">Product of the multiplication.</param>
	public static void Multiply(ref Matrix3X3 a, ref Matrix b, out Matrix3X3 result)
	{
		float m = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
		float m2 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
		float m3 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;
		float m4 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
		float m5 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
		float m6 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;
		float m7 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
		float m8 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
		float m9 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
	}

	/// <summary>
	/// Multiplies the two matrices.
	/// </summary>
	/// <param name="a">First matrix to multiply.</param>
	/// <param name="b">Second matrix to multiply.</param>
	/// <param name="result">Product of the multiplication.</param>
	public static void Multiply(ref Matrix a, ref Matrix3X3 b, out Matrix3X3 result)
	{
		float m = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
		float m2 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
		float m3 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;
		float m4 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
		float m5 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
		float m6 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;
		float m7 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
		float m8 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
		float m9 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
	}

	/// <summary>
	/// Multiplies a transposed matrix with another matrix.
	/// </summary>
	/// <param name="matrix">Matrix to be multiplied.</param>
	/// <param name="transpose">Matrix to be transposed and multiplied.</param>
	/// <param name="result">Product of the multiplication.</param>
	public static void MultiplyTransposed(ref Matrix3X3 transpose, ref Matrix3X3 matrix, out Matrix3X3 result)
	{
		float m = transpose.M11 * matrix.M11 + transpose.M21 * matrix.M21 + transpose.M31 * matrix.M31;
		float m2 = transpose.M11 * matrix.M12 + transpose.M21 * matrix.M22 + transpose.M31 * matrix.M32;
		float m3 = transpose.M11 * matrix.M13 + transpose.M21 * matrix.M23 + transpose.M31 * matrix.M33;
		float m4 = transpose.M12 * matrix.M11 + transpose.M22 * matrix.M21 + transpose.M32 * matrix.M31;
		float m5 = transpose.M12 * matrix.M12 + transpose.M22 * matrix.M22 + transpose.M32 * matrix.M32;
		float m6 = transpose.M12 * matrix.M13 + transpose.M22 * matrix.M23 + transpose.M32 * matrix.M33;
		float m7 = transpose.M13 * matrix.M11 + transpose.M23 * matrix.M21 + transpose.M33 * matrix.M31;
		float m8 = transpose.M13 * matrix.M12 + transpose.M23 * matrix.M22 + transpose.M33 * matrix.M32;
		float m9 = transpose.M13 * matrix.M13 + transpose.M23 * matrix.M23 + transpose.M33 * matrix.M33;
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
	}

	/// <summary>
	/// Multiplies a matrix with a transposed matrix.
	/// </summary>
	/// <param name="matrix">Matrix to be multiplied.</param>
	/// <param name="transpose">Matrix to be transposed and multiplied.</param>
	/// <param name="result">Product of the multiplication.</param>
	public static void MultiplyByTransposed(ref Matrix3X3 matrix, ref Matrix3X3 transpose, out Matrix3X3 result)
	{
		float m = matrix.M11 * transpose.M11 + matrix.M12 * transpose.M12 + matrix.M13 * transpose.M13;
		float m2 = matrix.M11 * transpose.M21 + matrix.M12 * transpose.M22 + matrix.M13 * transpose.M23;
		float m3 = matrix.M11 * transpose.M31 + matrix.M12 * transpose.M32 + matrix.M13 * transpose.M33;
		float m4 = matrix.M21 * transpose.M11 + matrix.M22 * transpose.M12 + matrix.M23 * transpose.M13;
		float m5 = matrix.M21 * transpose.M21 + matrix.M22 * transpose.M22 + matrix.M23 * transpose.M23;
		float m6 = matrix.M21 * transpose.M31 + matrix.M22 * transpose.M32 + matrix.M23 * transpose.M33;
		float m7 = matrix.M31 * transpose.M11 + matrix.M32 * transpose.M12 + matrix.M33 * transpose.M13;
		float m8 = matrix.M31 * transpose.M21 + matrix.M32 * transpose.M22 + matrix.M33 * transpose.M23;
		float m9 = matrix.M31 * transpose.M31 + matrix.M32 * transpose.M32 + matrix.M33 * transpose.M33;
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
	}

	/// <summary>
	/// Scales the matrix.
	/// </summary>
	/// <param name="matrix">Matrix to scale.</param>
	/// <param name="scale">Amount to scale.</param>
	/// <param name="result">Scaled matrix.</param>
	public static void Multiply(ref Matrix3X3 matrix, float scale, out Matrix3X3 result)
	{
		result.M11 = matrix.M11 * scale;
		result.M12 = matrix.M12 * scale;
		result.M13 = matrix.M13 * scale;
		result.M21 = matrix.M21 * scale;
		result.M22 = matrix.M22 * scale;
		result.M23 = matrix.M23 * scale;
		result.M31 = matrix.M31 * scale;
		result.M32 = matrix.M32 * scale;
		result.M33 = matrix.M33 * scale;
	}

	/// <summary>
	/// Negates every element in the matrix.
	/// </summary>
	/// <param name="matrix">Matrix to negate.</param>
	/// <param name="result">Negated matrix.</param>
	public static void Negate(ref Matrix3X3 matrix, out Matrix3X3 result)
	{
		result.M11 = 0f - matrix.M11;
		result.M12 = 0f - matrix.M12;
		result.M13 = 0f - matrix.M13;
		result.M21 = 0f - matrix.M21;
		result.M22 = 0f - matrix.M22;
		result.M23 = 0f - matrix.M23;
		result.M31 = 0f - matrix.M31;
		result.M32 = 0f - matrix.M32;
		result.M33 = 0f - matrix.M33;
	}

	/// <summary>
	/// Subtracts the two matrices from each other on a per-element basis.
	/// </summary>
	/// <param name="a">First matrix to subtract.</param>
	/// <param name="b">Second matrix to subtract.</param>
	/// <param name="result">Difference of the two matrices.</param>
	public static void Subtract(ref Matrix3X3 a, ref Matrix3X3 b, out Matrix3X3 result)
	{
		float m = a.M11 - b.M11;
		float m2 = a.M12 - b.M12;
		float m3 = a.M13 - b.M13;
		float m4 = a.M21 - b.M21;
		float m5 = a.M22 - b.M22;
		float m6 = a.M23 - b.M23;
		float m7 = a.M31 - b.M31;
		float m8 = a.M32 - b.M32;
		float m9 = a.M33 - b.M33;
		result.M11 = m;
		result.M12 = m2;
		result.M13 = m3;
		result.M21 = m4;
		result.M22 = m5;
		result.M23 = m6;
		result.M31 = m7;
		result.M32 = m8;
		result.M33 = m9;
	}

	/// <summary>
	/// Creates a 4x4 matrix from a 3x3 matrix.
	/// </summary>
	/// <param name="a">3x3 matrix.</param>
	/// <param name="b">Created 4x4 matrix.</param>
	public static void ToMatrix4X4(ref Matrix3X3 a, out Matrix b)
	{
		b = default(Matrix);
		b.M11 = a.M11;
		b.M12 = a.M12;
		b.M13 = a.M13;
		b.M21 = a.M21;
		b.M22 = a.M22;
		b.M23 = a.M23;
		b.M31 = a.M31;
		b.M32 = a.M32;
		b.M33 = a.M33;
		b.M44 = 1f;
		b.M14 = 0f;
		b.M24 = 0f;
		b.M34 = 0f;
		b.M41 = 0f;
		b.M42 = 0f;
		b.M43 = 0f;
	}

	/// <summary>
	/// Creates a 4x4 matrix from a 3x3 matrix.
	/// </summary>
	/// <param name="a">3x3 matrix.</param>
	/// <returns>Created 4x4 matrix.</returns>
	public static Matrix ToMatrix4X4(Matrix3X3 a)
	{
		return new Matrix
		{
			M11 = a.M11,
			M12 = a.M12,
			M13 = a.M13,
			M21 = a.M21,
			M22 = a.M22,
			M23 = a.M23,
			M31 = a.M31,
			M32 = a.M32,
			M33 = a.M33,
			M44 = 1f,
			M14 = 0f,
			M24 = 0f,
			M34 = 0f,
			M41 = 0f,
			M42 = 0f,
			M43 = 0f
		};
	}

	/// <summary>
	/// Transforms the vector by the matrix.
	/// </summary>
	/// <param name="v">Vector3 to transform.</param>
	/// <param name="matrix">Matrix to use as the transformation.</param>
	/// <returns>Product of the transformation.</returns>
	public static Vector3 Transform(Vector3 v, Matrix3X3 matrix)
	{
		Vector3 result = default(Vector3);
		float x = v.X;
		float y = v.Y;
		float z = v.Z;
		result.X = x * matrix.M11 + y * matrix.M21 + z * matrix.M31;
		result.Y = x * matrix.M12 + y * matrix.M22 + z * matrix.M32;
		result.Z = x * matrix.M13 + y * matrix.M23 + z * matrix.M33;
		return result;
	}

	/// <summary>
	/// Transforms the vector by the matrix.
	/// </summary>
	/// <param name="v">Vector3 to transform.</param>
	/// <param name="matrix">Matrix to use as the transformation.</param>
	/// <param name="result">Product of the transformation.</param>
	public static void Transform(ref Vector3 v, ref Matrix3X3 matrix, out Vector3 result)
	{
		float x = v.X;
		float y = v.Y;
		float z = v.Z;
		result = default(Vector3);
		result.X = x * matrix.M11 + y * matrix.M21 + z * matrix.M31;
		result.Y = x * matrix.M12 + y * matrix.M22 + z * matrix.M32;
		result.Z = x * matrix.M13 + y * matrix.M23 + z * matrix.M33;
	}

	/// <summary>
	/// Transforms the vector by the matrix.
	/// </summary>
	/// <param name="v">Vector3 to transform.</param>
	/// <param name="matrix">Matrix to use as the transformation.</param>
	/// <param name="result">Product of the transformation.</param>
	public static void Transform(ref Vector3 v, ref Matrix matrix, out Vector3 result)
	{
		float x = v.X;
		float y = v.Y;
		float z = v.Z;
		result = default(Vector3);
		result.X = x * matrix.M11 + y * matrix.M21 + z * matrix.M31;
		result.Y = x * matrix.M12 + y * matrix.M22 + z * matrix.M32;
		result.Z = x * matrix.M13 + y * matrix.M23 + z * matrix.M33;
	}

	/// <summary>
	/// Transforms the vector by the matrix's transpose.
	/// </summary>
	/// <param name="v">Vector3 to transform.</param>
	/// <param name="matrix">Matrix to use as the transformation transpose.</param>
	/// <returns>Product of the transformation.</returns>
	public static Vector3 TransformTranspose(Vector3 v, Matrix3X3 matrix)
	{
		float x = v.X;
		float y = v.Y;
		float z = v.Z;
		return new Vector3
		{
			X = x * matrix.M11 + y * matrix.M12 + z * matrix.M13,
			Y = x * matrix.M21 + y * matrix.M22 + z * matrix.M23,
			Z = x * matrix.M31 + y * matrix.M32 + z * matrix.M33
		};
	}

	/// <summary>
	/// Transforms the vector by the matrix's transpose.
	/// </summary>
	/// <param name="v">Vector3 to transform.</param>
	/// <param name="matrix">Matrix to use as the transformation transpose.</param>
	/// <param name="result">Product of the transformation.</param>
	public static void TransformTranspose(ref Vector3 v, ref Matrix3X3 matrix, out Vector3 result)
	{
		float x = v.X;
		float y = v.Y;
		float z = v.Z;
		result = default(Vector3);
		result.X = x * matrix.M11 + y * matrix.M12 + z * matrix.M13;
		result.Y = x * matrix.M21 + y * matrix.M22 + z * matrix.M23;
		result.Z = x * matrix.M31 + y * matrix.M32 + z * matrix.M33;
	}

	/// <summary>
	/// Transforms the vector by the matrix's transpose.
	/// </summary>
	/// <param name="v">Vector3 to transform.</param>
	/// <param name="matrix">Matrix to use as the transformation transpose.</param>
	/// <param name="result">Product of the transformation.</param>
	public static void TransformTranspose(ref Vector3 v, ref Matrix matrix, out Vector3 result)
	{
		float x = v.X;
		float y = v.Y;
		float z = v.Z;
		result = default(Vector3);
		result.X = x * matrix.M11 + y * matrix.M12 + z * matrix.M13;
		result.Y = x * matrix.M21 + y * matrix.M22 + z * matrix.M23;
		result.Z = x * matrix.M31 + y * matrix.M32 + z * matrix.M33;
	}

	/// <summary>
	/// Computes the transposed matrix of a matrix.
	/// </summary>
	/// <param name="matrix">Matrix to transpose.</param>
	/// <param name="result">Transposed matrix.</param>
	public static void Transpose(ref Matrix3X3 matrix, out Matrix3X3 result)
	{
		float m = matrix.M12;
		float m2 = matrix.M13;
		float m3 = matrix.M21;
		float m4 = matrix.M23;
		float m5 = matrix.M31;
		float m6 = matrix.M32;
		result.M11 = matrix.M11;
		result.M12 = m3;
		result.M13 = m5;
		result.M21 = m;
		result.M22 = matrix.M22;
		result.M23 = m6;
		result.M31 = m2;
		result.M32 = m4;
		result.M33 = matrix.M33;
	}

	/// <summary>
	/// Computes the transposed matrix of a matrix.
	/// </summary>
	/// <param name="matrix">Matrix to transpose.</param>
	/// <param name="result">Transposed matrix.</param>
	public static void Transpose(ref Matrix matrix, out Matrix3X3 result)
	{
		float m = matrix.M12;
		float m2 = matrix.M13;
		float m3 = matrix.M21;
		float m4 = matrix.M23;
		float m5 = matrix.M31;
		float m6 = matrix.M32;
		result.M11 = matrix.M11;
		result.M12 = m3;
		result.M13 = m5;
		result.M21 = m;
		result.M22 = matrix.M22;
		result.M23 = m6;
		result.M31 = m2;
		result.M32 = m4;
		result.M33 = matrix.M33;
	}

	/// <summary>
	/// Creates a string representation of the matrix.
	/// </summary>
	/// <returns>A string representation of the matrix.</returns>
	public override string ToString()
	{
		return "{" + M11 + ", " + M12 + ", " + M13 + "} {" + M21 + ", " + M22 + ", " + M23 + "} {" + M31 + ", " + M32 + ", " + M33 + "}";
	}

	/// <summary>
	/// Calculates the determinant of the matrix.
	/// </summary>
	/// <returns>The matrix's determinant.</returns>
	public float Determinant()
	{
		return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 - M31 * M22 * M13 - M32 * M23 * M11 - M33 * M21 * M12;
	}

	/// <summary>
	/// Calculates the determinant of largest nonsingular submatrix, excluding 2x2's that involve M13 or M31, and excluding all 1x1's that involve nondiagonal elements.
	/// </summary>
	/// <param name="subMatrixCode">Represents the submatrix that was used to compute the determinant.
	/// 0 is the full 3x3.  1 is the upper left 2x2.  2 is the lower right 2x2.  3 is the four corners.
	/// 4 is M11.  5 is M22.  6 is M33.</param>
	/// <returns>The matrix's determinant.</returns>
	internal float AdaptiveDeterminant(out int subMatrixCode)
	{
		float num = M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 - M31 * M22 * M13 - M32 * M23 * M11 - M33 * M21 * M12;
		if (num != 0f)
		{
			subMatrixCode = 0;
			return num;
		}
		num = M11 * M22 - M12 * M21;
		if (num != 0f)
		{
			subMatrixCode = 1;
			return num;
		}
		num = M22 * M33 - M23 * M32;
		if (num != 0f)
		{
			subMatrixCode = 2;
			return num;
		}
		num = M11 * M33 - M13 * M12;
		if (num != 0f)
		{
			subMatrixCode = 3;
			return num;
		}
		if (M11 != 0f)
		{
			subMatrixCode = 4;
			return M11;
		}
		if (M22 != 0f)
		{
			subMatrixCode = 5;
			return M22;
		}
		if (M33 != 0f)
		{
			subMatrixCode = 6;
			return M33;
		}
		subMatrixCode = -1;
		return 0f;
	}

	/// <summary>
	/// Constructs a quaternion from a 3x3 rotation matrix.
	/// </summary>
	/// <param name="r">Rotation matrix to create the quaternion from.</param>
	/// <param name="q">Quaternion based on the rotation matrix.</param>
	public static void CreateQuaternion(ref Matrix3X3 r, out Quaternion q)
	{
		float num = r.M11 + r.M22 + r.M33;
		q = default(Quaternion);
		if (num >= 0f)
		{
			float num2 = (float)Math.Sqrt((double)num + 1.0) * 2f;
			float num3 = 1f / num2;
			q.W = 0.25f * num2;
			q.X = (r.M23 - r.M32) * num3;
			q.Y = (r.M31 - r.M13) * num3;
			q.Z = (r.M12 - r.M21) * num3;
		}
		else if ((r.M11 > r.M22) & (r.M11 > r.M33))
		{
			float num4 = (float)Math.Sqrt(1.0 + (double)r.M11 - (double)r.M22 - (double)r.M33) * 2f;
			float num5 = 1f / num4;
			q.W = (r.M23 - r.M32) * num5;
			q.X = 0.25f * num4;
			q.Y = (r.M21 + r.M12) * num5;
			q.Z = (r.M31 + r.M13) * num5;
		}
		else if (r.M22 > r.M33)
		{
			float num6 = (float)Math.Sqrt(1.0 + (double)r.M22 - (double)r.M11 - (double)r.M33) * 2f;
			float num7 = 1f / num6;
			q.W = (r.M31 - r.M13) * num7;
			q.X = (r.M21 + r.M12) * num7;
			q.Y = 0.25f * num6;
			q.Z = (r.M32 + r.M23) * num7;
		}
		else
		{
			float num8 = (float)Math.Sqrt(1.0 + (double)r.M33 - (double)r.M11 - (double)r.M22) * 2f;
			float num9 = 1f / num8;
			q.W = (r.M12 - r.M21) * num9;
			q.X = (r.M31 + r.M13) * num9;
			q.Y = (r.M32 + r.M23) * num9;
			q.Z = 0.25f * num8;
		}
	}

	/// <summary>
	/// Creates a 3x3 matrix representing the orientation stored in the quaternion.
	/// </summary>
	/// <param name="quaternion">Quaternion to use to create a matrix.</param>
	/// <param name="result">Matrix representing the quaternion's orientation.</param>
	public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix3X3 result)
	{
		float num = 2f * quaternion.X * quaternion.X;
		float num2 = 2f * quaternion.Y * quaternion.Y;
		float num3 = 2f * quaternion.Z * quaternion.Z;
		float num4 = 2f * quaternion.X * quaternion.Y;
		float num5 = 2f * quaternion.X * quaternion.Z;
		float num6 = 2f * quaternion.X * quaternion.W;
		float num7 = 2f * quaternion.Y * quaternion.Z;
		float num8 = 2f * quaternion.Y * quaternion.W;
		float num9 = 2f * quaternion.Z * quaternion.W;
		result.M11 = 1f - num2 - num3;
		result.M21 = num4 - num9;
		result.M31 = num5 + num8;
		result.M12 = num4 + num9;
		result.M22 = 1f - num - num3;
		result.M32 = num7 - num6;
		result.M13 = num5 - num8;
		result.M23 = num7 + num6;
		result.M33 = 1f - num - num2;
	}

	/// <summary>
	/// Computes the outer product of the given vectors.
	/// </summary>
	/// <param name="a">First vector.</param>
	/// <param name="b">Second vector.</param>
	/// <param name="result">Outer product result.</param>
	public static void CreateOuterProduct(ref Vector3 a, ref Vector3 b, out Matrix3X3 result)
	{
		result.M11 = a.X * b.X;
		result.M12 = a.X * b.Y;
		result.M13 = a.X * b.Z;
		result.M21 = a.Y * b.X;
		result.M22 = a.Y * b.Y;
		result.M23 = a.Y * b.Z;
		result.M31 = a.Z * b.X;
		result.M32 = a.Z * b.Y;
		result.M33 = a.Z * b.Z;
	}

	/// <summary>
	/// Creates a matrix representing a rotation of a given angle around a given axis.
	/// </summary>
	/// <param name="axis">Axis around which to rotate.</param>
	/// <param name="angle">Amount to rotate.</param>
	/// <returns>Matrix representing the rotation.</returns>
	public static Matrix3X3 CreateFromAxisAngle(Vector3 axis, float angle)
	{
		CreateFromAxisAngle(ref axis, angle, out var result);
		return result;
	}

	/// <summary>
	/// Creates a matrix representing a rotation of a given angle around a given axis.
	/// </summary>
	/// <param name="axis">Axis around which to rotate.</param>
	/// <param name="angle">Amount to rotate.</param>
	/// <param name="result">Matrix representing the rotation.</param>
	public static void CreateFromAxisAngle(ref Vector3 axis, float angle, out Matrix3X3 result)
	{
		float num = axis.X * axis.X;
		float num2 = axis.Y * axis.Y;
		float num3 = axis.Z * axis.Z;
		float num4 = axis.X * axis.Y;
		float num5 = axis.X * axis.Z;
		float num6 = axis.Y * axis.Z;
		float num7 = (float)Math.Sin(angle);
		float num8 = 1f - (float)Math.Cos(angle);
		result.M11 = 1f + num8 * (num - 1f);
		result.M21 = (0f - axis.Z) * num7 + num8 * num4;
		result.M31 = axis.Y * num7 + num8 * num5;
		result.M12 = axis.Z * num7 + num8 * num4;
		result.M22 = 1f + num8 * (num2 - 1f);
		result.M32 = (0f - axis.X) * num7 + num8 * num6;
		result.M13 = (0f - axis.Y) * num7 + num8 * num5;
		result.M23 = axis.X * num7 + num8 * num6;
		result.M33 = 1f + num8 * (num3 - 1f);
	}
}
