using System;
using E;
using Microsoft.Xna.Framework;
using d;
using r;

namespace n
{
	internal class _7 : r._6
	{
		internal enum _00065h : byte
		{
			Position,
			Orientation,
			LinearVelocity,
			AngularVelocity
		}

		internal struct _00065b
		{
			internal Quaternion a5h;

			internal Vector3 a5b;

			internal _00065h a56;

			internal E.h a5a;
		}

		private d.h<_00065b> a5h = new d.h<_00065b>();

		public _7()
		{
			Enabled = true;
		}

		public void EnqueuePosition(E.h entity, ref Vector3 newPosition)
		{
			a5h.Enqueue(new _00065b
			{
				a5a = entity,
				a5b = newPosition,
				a56 = _00065h.Position
			});
		}

		public void EnqueueOrientation(E.h entity, ref Quaternion newOrientationQuaternion)
		{
			a5h.Enqueue(new _00065b
			{
				a5a = entity,
				a5h = newOrientationQuaternion,
				a56 = _00065h.Orientation
			});
		}

		public void EnqueueLinearVelocity(E.h entity, ref Vector3 newLinearVelocity)
		{
			a5h.Enqueue(new _00065b
			{
				a5a = entity,
				a5b = newLinearVelocity,
				a56 = _00065h.LinearVelocity
			});
		}

		public void EnqueueAngularVelocity(E.h entity, ref Vector3 newAngularVelocity)
		{
			a5h.Enqueue(new _00065b
			{
				a5a = entity,
				a5b = newAngularVelocity,
				a56 = _00065h.AngularVelocity
			});
		}

		protected override void UpdateStage()
		{
			_00065b item;
			while (a5h.TryDequeueFirst(out item))
			{
				E.h a5a = item.a5a;
				switch (item.a56)
				{
				case _00065h.Position:
					a5a.Position = item.a5b;
					break;
				case _00065h.Orientation:
					a5a.Orientation = item.a5h;
					break;
				case _00065h.LinearVelocity:
					a5a.LinearVelocity = item.a5b;
					break;
				case _00065h.AngularVelocity:
					a5a.AngularVelocity = item.a5b;
					break;
				}
			}
		}
	}
}
namespace N
{
	internal struct _7(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
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

		public static _7 Identity => new _7(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

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

		public static void Add(ref _7 a, ref _7 b, out _7 result)
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

		public static void Add(ref Matrix a, ref _7 b, out _7 result)
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

		public static void Add(ref _7 a, ref Matrix b, out _7 result)
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

		public static void Add(ref Matrix a, ref Matrix b, out _7 result)
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

		public static void CreateCrossProduct(ref Vector3 v, out _7 result)
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

		public static void CreateFromMatrix(ref Matrix matrix4X4, out _7 matrix3X3)
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

		public static _7 CreateFromMatrix(Matrix matrix4X4)
		{
			_7 result = default(_7);
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

		public static void CreateScale(float scale, out _7 matrix)
		{
			matrix = default(_7);
			matrix.M11 = scale;
			matrix.M22 = scale;
			matrix.M33 = scale;
		}

		public static _7 CreateScale(float scale)
		{
			return new _7
			{
				M11 = scale,
				M22 = scale,
				M33 = scale
			};
		}

		public static void CreateScale(ref Vector3 scale, out _7 matrix)
		{
			matrix = default(_7);
			matrix.M11 = scale.X;
			matrix.M22 = scale.Y;
			matrix.M33 = scale.Z;
		}

		public static _7 CreateScale(ref Vector3 scale)
		{
			return new _7
			{
				M11 = scale.X,
				M22 = scale.Y,
				M33 = scale.Z
			};
		}

		public static void CreateScale(float x, float y, float z, out _7 matrix)
		{
			matrix = default(_7);
			matrix.M11 = x;
			matrix.M22 = y;
			matrix.M33 = z;
		}

		public static _7 CreateScale(float x, float y, float z)
		{
			return new _7
			{
				M11 = x,
				M22 = y,
				M33 = z
			};
		}

		public static void Invert(ref _7 matrix, out _7 result)
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

		internal static void _6_0015(ref _7 P_0, out _7 P_1)
		{
			float num = 1f / P_0._6_0016(out var num2);
			float m;
			float m2;
			float m3;
			float m4;
			float m5;
			float m6;
			float m7;
			float m8;
			float m9;
			switch (num2)
			{
			case 0:
				m = (P_0.M22 * P_0.M33 - P_0.M23 * P_0.M32) * num;
				m2 = (P_0.M13 * P_0.M32 - P_0.M33 * P_0.M12) * num;
				m3 = (P_0.M12 * P_0.M23 - P_0.M22 * P_0.M13) * num;
				m4 = (P_0.M23 * P_0.M31 - P_0.M21 * P_0.M33) * num;
				m5 = (P_0.M11 * P_0.M33 - P_0.M13 * P_0.M31) * num;
				m6 = (P_0.M13 * P_0.M21 - P_0.M11 * P_0.M23) * num;
				m7 = (P_0.M21 * P_0.M32 - P_0.M22 * P_0.M31) * num;
				m8 = (P_0.M12 * P_0.M31 - P_0.M11 * P_0.M32) * num;
				m9 = (P_0.M11 * P_0.M22 - P_0.M12 * P_0.M21) * num;
				break;
			case 1:
				m = P_0.M22 * num;
				m2 = (0f - P_0.M12) * num;
				m3 = 0f;
				m4 = (0f - P_0.M21) * num;
				m5 = P_0.M11 * num;
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
				m5 = P_0.M33 * num;
				m6 = (0f - P_0.M23) * num;
				m7 = 0f;
				m8 = (0f - P_0.M32) * num;
				m9 = P_0.M22 * num;
				break;
			case 3:
				m = P_0.M33 * num;
				m2 = 0f;
				m3 = (0f - P_0.M13) * num;
				m4 = 0f;
				m5 = 0f;
				m6 = 0f;
				m7 = (0f - P_0.M31) * num;
				m8 = 0f;
				m9 = P_0.M11 * num;
				break;
			case 4:
				m = 1f / P_0.M11;
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
				m5 = 1f / P_0.M22;
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
				m9 = 1f / P_0.M33;
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
			P_1.M11 = m;
			P_1.M12 = m2;
			P_1.M13 = m3;
			P_1.M21 = m4;
			P_1.M22 = m5;
			P_1.M23 = m6;
			P_1.M31 = m7;
			P_1.M32 = m8;
			P_1.M33 = m9;
		}

		public static _7 operator *(_7 a, _7 b)
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
			_7 result = default(_7);
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

		public static void Multiply(ref _7 a, ref _7 b, out _7 result)
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

		public static void Multiply(ref _7 a, ref Matrix b, out _7 result)
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

		public static void Multiply(ref Matrix a, ref _7 b, out _7 result)
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

		public static void MultiplyTransposed(ref _7 transpose, ref _7 matrix, out _7 result)
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

		public static void MultiplyByTransposed(ref _7 matrix, ref _7 transpose, out _7 result)
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

		public static void Multiply(ref _7 matrix, float scale, out _7 result)
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

		public static void Negate(ref _7 matrix, out _7 result)
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

		public static void Subtract(ref _7 a, ref _7 b, out _7 result)
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

		public static void ToMatrix4X4(ref _7 a, out Matrix b)
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

		public static Matrix ToMatrix4X4(_7 a)
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

		public static Vector3 Transform(Vector3 v, _7 matrix)
		{
			Vector3 result = default(Vector3);
			float num = v.X;
			float y = v.Y;
			float z = v.Z;
			result.X = num * matrix.M11 + y * matrix.M21 + z * matrix.M31;
			result.Y = num * matrix.M12 + y * matrix.M22 + z * matrix.M32;
			result.Z = num * matrix.M13 + y * matrix.M23 + z * matrix.M33;
			return result;
		}

		public static void Transform(ref Vector3 v, ref _7 matrix, out Vector3 result)
		{
			float num = v.X;
			float y = v.Y;
			float z = v.Z;
			result = default(Vector3);
			result.X = num * matrix.M11 + y * matrix.M21 + z * matrix.M31;
			result.Y = num * matrix.M12 + y * matrix.M22 + z * matrix.M32;
			result.Z = num * matrix.M13 + y * matrix.M23 + z * matrix.M33;
		}

		public static void Transform(ref Vector3 v, ref Matrix matrix, out Vector3 result)
		{
			float num = v.X;
			float y = v.Y;
			float z = v.Z;
			result = default(Vector3);
			result.X = num * matrix.M11 + y * matrix.M21 + z * matrix.M31;
			result.Y = num * matrix.M12 + y * matrix.M22 + z * matrix.M32;
			result.Z = num * matrix.M13 + y * matrix.M23 + z * matrix.M33;
		}

		public static Vector3 TransformTranspose(Vector3 v, _7 matrix)
		{
			float num = v.X;
			float y = v.Y;
			float z = v.Z;
			return new Vector3
			{
				X = num * matrix.M11 + y * matrix.M12 + z * matrix.M13,
				Y = num * matrix.M21 + y * matrix.M22 + z * matrix.M23,
				Z = num * matrix.M31 + y * matrix.M32 + z * matrix.M33
			};
		}

		public static void TransformTranspose(ref Vector3 v, ref _7 matrix, out Vector3 result)
		{
			float num = v.X;
			float y = v.Y;
			float z = v.Z;
			result = default(Vector3);
			result.X = num * matrix.M11 + y * matrix.M12 + z * matrix.M13;
			result.Y = num * matrix.M21 + y * matrix.M22 + z * matrix.M23;
			result.Z = num * matrix.M31 + y * matrix.M32 + z * matrix.M33;
		}

		public static void TransformTranspose(ref Vector3 v, ref Matrix matrix, out Vector3 result)
		{
			float num = v.X;
			float y = v.Y;
			float z = v.Z;
			result = default(Vector3);
			result.X = num * matrix.M11 + y * matrix.M12 + z * matrix.M13;
			result.Y = num * matrix.M21 + y * matrix.M22 + z * matrix.M23;
			result.Z = num * matrix.M31 + y * matrix.M32 + z * matrix.M33;
		}

		public static void Transpose(ref _7 matrix, out _7 result)
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

		public static void Transpose(ref Matrix matrix, out _7 result)
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

		public override string ToString()
		{
			return "{" + M11 + ", " + M12 + ", " + M13 + "} {" + M21 + ", " + M22 + ", " + M23 + "} {" + M31 + ", " + M32 + ", " + M33 + "}";
		}

		public float Determinant()
		{
			return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 - M31 * M22 * M13 - M32 * M23 * M11 - M33 * M21 * M12;
		}

		internal float _6_0016(out int P_0)
		{
			float num = M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 - M31 * M22 * M13 - M32 * M23 * M11 - M33 * M21 * M12;
			if (num != 0f)
			{
				P_0 = 0;
				return num;
			}
			num = M11 * M22 - M12 * M21;
			if (num != 0f)
			{
				P_0 = 1;
				return num;
			}
			num = M22 * M33 - M23 * M32;
			if (num != 0f)
			{
				P_0 = 2;
				return num;
			}
			num = M11 * M33 - M13 * M12;
			if (num != 0f)
			{
				P_0 = 3;
				return num;
			}
			if (M11 != 0f)
			{
				P_0 = 4;
				return M11;
			}
			if (M22 != 0f)
			{
				P_0 = 5;
				return M22;
			}
			if (M33 != 0f)
			{
				P_0 = 6;
				return M33;
			}
			P_0 = -1;
			return 0f;
		}

		public static void CreateQuaternion(ref _7 r, out Quaternion q)
		{
			float num = r.M11 + r.M22 + r.M33;
			q = default(Quaternion);
			if (num > 0f)
			{
				float num2 = (float)Math.Sqrt((double)num + 1.0) * 2f;
				float num3 = 1f / num2;
				q.W = 0.25f * num2;
				q.X = (r.M32 - r.M23) * num3;
				q.Y = (r.M13 - r.M31) * num3;
				q.Z = (r.M21 - r.M12) * num3;
			}
			else if ((r.M11 > r.M22) & (r.M11 > r.M33))
			{
				float num4 = (float)Math.Sqrt(1.0 + (double)r.M11 - (double)r.M22 - (double)r.M33) * 2f;
				float num5 = 1f / num4;
				q.W = (r.M32 - r.M23) * num5;
				q.X = 0.25f * num4;
				q.Y = (r.M12 + r.M21) * num5;
				q.Z = (r.M13 + r.M31) * num5;
			}
			else if (r.M22 > r.M33)
			{
				float num6 = (float)Math.Sqrt(1.0 + (double)r.M22 - (double)r.M11 - (double)r.M33) * 2f;
				float num7 = 1f / num6;
				q.W = (r.M13 - r.M31) * num7;
				q.X = (r.M12 + r.M21) * num7;
				q.Y = 0.25f * num6;
				q.Z = (r.M23 + r.M32) * num7;
			}
			else
			{
				float num8 = (float)Math.Sqrt(1.0 + (double)r.M33 - (double)r.M11 - (double)r.M22) * 2f;
				float num9 = 1f / num8;
				q.W = (r.M21 - r.M12) * num9;
				q.X = (r.M13 + r.M31) * num9;
				q.Y = (r.M23 + r.M32) * num9;
				q.Z = 0.25f * num8;
			}
		}

		public static void CreateFromQuaternion(ref Quaternion quaternion, out _7 result)
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

		public static void CreateOuterProduct(ref Vector3 a, ref Vector3 b, out _7 result)
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

		public static _7 CreateFromAxisAngle(Vector3 axis, float angle)
		{
			CreateFromAxisAngle(ref axis, angle, out var result);
			return result;
		}

		public static void CreateFromAxisAngle(ref Vector3 axis, float angle, out _7 result)
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
}
