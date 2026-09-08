using System.Runtime.CompilerServices;
using E;
using Microsoft.Xna.Framework;

namespace n
{
	internal class _6
	{
		internal int a5h;

		[CompilerGenerated]
		private b a5b;

		[CompilerGenerated]
		private h a56;

		[CompilerGenerated]
		private _0006 a5a;

		[CompilerGenerated]
		private E.h a57;

		public b BufferedStatesManager
		{
			[CompilerGenerated]
			get
			{
				return a5b;
			}
			[CompilerGenerated]
			internal set
			{
				a5b = b2;
			}
		}

		public h States
		{
			[CompilerGenerated]
			get
			{
				return a56;
			}
			[CompilerGenerated]
			private set
			{
				a56 = h2;
			}
		}

		public _0006 InterpolatedStates
		{
			[CompilerGenerated]
			get
			{
				return a5a;
			}
			[CompilerGenerated]
			private set
			{
				a5a = obj;
			}
		}

		public int MotionStateIndex
		{
			get
			{
				return a5h;
			}
			internal set
			{
				a5h = num;
			}
		}

		public E.h Entity
		{
			[CompilerGenerated]
			get
			{
				return a57;
			}
			[CompilerGenerated]
			private set
			{
				a57 = h2;
			}
		}

		public _6(E.h entity)
		{
			Entity = entity;
			States = new h(this);
			InterpolatedStates = new _0006(this);
		}
	}
}
namespace N
{
	internal struct _6(float m11, float m12, float m13, float m21, float m22, float m23)
	{
		public float M11 = m11;

		public float M12 = m12;

		public float M13 = m13;

		public float M21 = m21;

		public float M22 = m22;

		public float M23 = m23;

		public static void Add(ref _6 a, ref _6 b, out _6 result)
		{
			float m = a.M11 + b.M11;
			float m2 = a.M12 + b.M12;
			float m3 = a.M13 + b.M13;
			float m4 = a.M21 + b.M21;
			float m5 = a.M22 + b.M22;
			float m6 = a.M23 + b.M23;
			result.M11 = m;
			result.M12 = m2;
			result.M13 = m3;
			result.M21 = m4;
			result.M22 = m5;
			result.M23 = m6;
		}

		public static void Multiply(ref _6 a, ref _7 b, out _6 result)
		{
			float m = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
			float m2 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
			float m3 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;
			float m4 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
			float m5 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
			float m6 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;
			result.M11 = m;
			result.M12 = m2;
			result.M13 = m3;
			result.M21 = m4;
			result.M22 = m5;
			result.M23 = m6;
		}

		public static void Multiply(ref _6 a, ref Matrix b, out _6 result)
		{
			float m = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
			float m2 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
			float m3 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;
			float m4 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
			float m5 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
			float m6 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;
			result.M11 = m;
			result.M12 = m2;
			result.M13 = m3;
			result.M21 = m4;
			result.M22 = m5;
			result.M23 = m6;
		}

		public static void Negate(ref _6 matrix, out _6 result)
		{
			float m = 0f - matrix.M11;
			float m2 = 0f - matrix.M12;
			float m3 = 0f - matrix.M13;
			float m4 = 0f - matrix.M21;
			float m5 = 0f - matrix.M22;
			float m6 = 0f - matrix.M23;
			result.M11 = m;
			result.M12 = m2;
			result.M13 = m3;
			result.M21 = m4;
			result.M22 = m5;
			result.M23 = m6;
		}

		public static void Subtract(ref _6 a, ref _6 b, out _6 result)
		{
			float m = a.M11 - b.M11;
			float m2 = a.M12 - b.M12;
			float m3 = a.M13 - b.M13;
			float m4 = a.M21 - b.M21;
			float m5 = a.M22 - b.M22;
			float m6 = a.M23 - b.M23;
			result.M11 = m;
			result.M12 = m2;
			result.M13 = m3;
			result.M21 = m4;
			result.M22 = m5;
			result.M23 = m6;
		}

		public static void Transform(ref Vector2 v, ref _6 matrix, out Vector3 result)
		{
			result = default(Vector3);
			result.X = v.X * matrix.M11 + v.Y * matrix.M21;
			result.Y = v.X * matrix.M12 + v.Y * matrix.M22;
			result.Z = v.X * matrix.M13 + v.Y * matrix.M23;
		}

		public static void Transform(ref Vector3 v, ref _6 matrix, out Vector2 result)
		{
			result = default(Vector2);
			result.X = matrix.M11 * v.X + matrix.M12 * v.Y + matrix.M13 * v.Z;
			result.Y = matrix.M21 * v.X + matrix.M22 * v.Y + matrix.M23 * v.Z;
		}

		public static void Transpose(ref _6 matrix, out a result)
		{
			result.M11 = matrix.M11;
			result.M12 = matrix.M21;
			result.M21 = matrix.M12;
			result.M22 = matrix.M22;
			result.M31 = matrix.M13;
			result.M32 = matrix.M23;
		}

		public override string ToString()
		{
			return "{" + M11 + ", " + M12 + ", " + M13 + "} {" + M21 + ", " + M22 + ", " + M23 + "}";
		}
	}
}
