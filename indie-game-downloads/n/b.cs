using System;
using System.Runtime.CompilerServices;
using E;
using Microsoft.Xna.Framework;
using d;
using l;

namespace n
{
	internal class b
	{
		internal l._7<E.h> a5h = new l._7<E.h>();

		private bool a5b;

		[CompilerGenerated]
		private a a56;

		[CompilerGenerated]
		private v a5a;

		public a ReadBuffers
		{
			[CompilerGenerated]
			get
			{
				return a56;
			}
			[CompilerGenerated]
			private set
			{
				a56 = a2;
			}
		}

		public v InterpolatedStates
		{
			[CompilerGenerated]
			get
			{
				return a5a;
			}
			[CompilerGenerated]
			private set
			{
				a5a = v2;
			}
		}

		public l.X<E.h> Entities => new l.X<E.h>(a5h);

		public bool Enabled
		{
			get
			{
				return a5b;
			}
			set
			{
				if (!a5b && value)
				{
					ReadBuffers.Enabled = true;
					InterpolatedStates.Enabled = true;
				}
				else if (a5b && !value)
				{
					InterpolatedStates.Enabled = false;
					ReadBuffers.Enabled = false;
				}
				a5b = value;
			}
		}

		public b()
		{
			InterpolatedStates = new v(this);
			ReadBuffers = new a(this);
		}

		public b(d.b threadManager)
		{
			InterpolatedStates = new v(this, threadManager);
			ReadBuffers = new a(this, threadManager);
		}

		public void Add(E.h e)
		{
			lock (InterpolatedStates.FlipLocker)
			{
				lock (ReadBuffers.FlipLocker)
				{
					if (e.BufferedStates.BufferedStatesManager == null)
					{
						e.BufferedStates.BufferedStatesManager = this;
						e.BufferedStates.a5h = a5h.Count;
						a5h.Add(e);
						if (ReadBuffers.Enabled)
						{
							ReadBuffers._64(e);
						}
						if (InterpolatedStates.Enabled)
						{
							InterpolatedStates._64(e);
						}
						return;
					}
					throw new InvalidOperationException("Entity already belongs to a BufferedStatesManager; cannot add.");
				}
			}
		}

		public void Remove(E.h e)
		{
			lock (InterpolatedStates.FlipLocker)
			{
				lock (ReadBuffers.FlipLocker)
				{
					if (e.BufferedStates.BufferedStatesManager == this)
					{
						int num = a5h.IndexOf(e);
						int num2 = a5h.Count - 1;
						a5h[num] = a5h[num2];
						a5h.RemoveAt(num2);
						if (num < a5h.Count)
						{
							a5h[num].BufferedStates.a5h = num;
						}
						if (ReadBuffers.Enabled)
						{
							ReadBuffers._6e(num, num2);
						}
						if (InterpolatedStates.Enabled)
						{
							InterpolatedStates._6e(num, num2);
						}
						e.BufferedStates.BufferedStatesManager = null;
						return;
					}
					throw new InvalidOperationException("Entity does not belong to this BufferedStatesManager; cannot remove.");
				}
			}
		}
	}
	internal struct B : IEquatable<B>
	{
		public Vector3 Position;

		public Quaternion Orientation;

		public Vector3 LinearVelocity;

		public Vector3 AngularVelocity;

		public Matrix OrientationMatrix
		{
			get
			{
				Matrix.CreateFromQuaternion(ref Orientation, out var result);
				return result;
			}
		}

		public Matrix WorldTransform
		{
			get
			{
				Matrix.CreateFromQuaternion(ref Orientation, out var result);
				result.Translation = Position;
				return result;
			}
		}

		public bool Equals(B other)
		{
			if (other.AngularVelocity == AngularVelocity && other.LinearVelocity == LinearVelocity && other.Position == Position)
			{
				return other.Orientation == Orientation;
			}
			return false;
		}
	}
}
namespace N
{
	internal struct b(float m11, float m12, float m21, float m22)
	{
		public float M11 = m11;

		public float M12 = m12;

		public float M21 = m21;

		public float M22 = m22;

		public static b Identity => new b(1f, 0f, 1f, 0f);

		public static void Add(ref b a, ref b b, out b result)
		{
			float m = a.M11 + b.M11;
			float m2 = a.M12 + b.M12;
			float m3 = a.M21 + b.M21;
			float m4 = a.M22 + b.M22;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
		}

		public static void Add(ref Matrix a, ref b b, out b result)
		{
			float m = a.M11 + b.M11;
			float m2 = a.M12 + b.M12;
			float m3 = a.M21 + b.M21;
			float m4 = a.M22 + b.M22;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
		}

		public static void Add(ref b a, ref Matrix b, out b result)
		{
			float m = a.M11 + b.M11;
			float m2 = a.M12 + b.M12;
			float m3 = a.M21 + b.M21;
			float m4 = a.M22 + b.M22;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
		}

		public static void Add(ref Matrix a, ref Matrix b, out b result)
		{
			float m = a.M11 + b.M11;
			float m2 = a.M12 + b.M12;
			float m3 = a.M21 + b.M21;
			float m4 = a.M22 + b.M22;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
		}

		public static void CreateScale(float scale, out b matrix)
		{
			matrix.M11 = scale;
			matrix.M22 = scale;
			matrix.M12 = 0f;
			matrix.M21 = 0f;
		}

		public static void Invert(ref b matrix, out b result)
		{
			float num = 1f / (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21);
			float m = matrix.M22 * num;
			float m2 = (0f - matrix.M12) * num;
			float m3 = (0f - matrix.M21) * num;
			float m4 = matrix.M11 * num;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
		}

		public static void Multiply(ref b a, ref b b, out b result)
		{
			float m = a.M11 * b.M11 + a.M12 * b.M21;
			float m2 = a.M11 * b.M12 + a.M12 * b.M22;
			float m3 = a.M21 * b.M11 + a.M22 * b.M21;
			float m4 = a.M21 * b.M12 + a.M22 * b.M22;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
		}

		public static void Multiply(ref b a, ref Matrix b, out b result)
		{
			float m = a.M11 * b.M11 + a.M12 * b.M21;
			float m2 = a.M11 * b.M12 + a.M12 * b.M22;
			float m3 = a.M21 * b.M11 + a.M22 * b.M21;
			float m4 = a.M21 * b.M12 + a.M22 * b.M22;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
		}

		public static void Multiply(ref Matrix a, ref b b, out b result)
		{
			float m = a.M11 * b.M11 + a.M12 * b.M21;
			float m2 = a.M11 * b.M12 + a.M12 * b.M22;
			float m3 = a.M21 * b.M11 + a.M22 * b.M21;
			float m4 = a.M21 * b.M12 + a.M22 * b.M22;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
		}

		public static void Multiply(ref _6 a, ref a b, out b result)
		{
			result.M11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
			result.M12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
			result.M21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
			result.M22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
		}

		public static void Negate(ref b matrix, out b result)
		{
			float m = 0f - matrix.M11;
			float m2 = 0f - matrix.M12;
			float m3 = 0f - matrix.M21;
			float m4 = 0f - matrix.M22;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
		}

		public static void Subtract(ref b a, ref b b, out b result)
		{
			float m = a.M11 - b.M11;
			float m2 = a.M12 - b.M12;
			float m3 = a.M21 - b.M21;
			float m4 = a.M22 - b.M22;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
		}

		public static void Transform(ref Vector2 v, ref b matrix, out Vector2 result)
		{
			float num = v.X;
			float num2 = v.Y;
			result = default(Vector2);
			result.X = num * matrix.M11 + num2 * matrix.M21;
			result.Y = num * matrix.M12 + num2 * matrix.M22;
		}

		public static void Transpose(ref b matrix, out b result)
		{
			float m = matrix.M12;
			result.M11 = matrix.M11;
			result.M12 = matrix.M21;
			result.M21 = m;
			result.M22 = matrix.M22;
		}

		public override string ToString()
		{
			return "{" + M11 + ", " + M12 + "} {" + M21 + ", " + M22 + "}";
		}

		public float Determinant()
		{
			return M11 * M22 - M12 * M21;
		}
	}
}
