using System;
using System.Runtime.CompilerServices;
using E;
using Microsoft.Xna.Framework;
using d;
using r;

namespace n
{
	internal class a : r.b
	{
		private b a5h;

		internal B[] a5b;

		internal B[] a56;

		private Action<int> a5a;

		[CompilerGenerated]
		private object a57;

		public override bool Enabled
		{
			get
			{
				return base.Enabled;
			}
			set
			{
				if (base.Enabled && !value)
				{
					if (!a5h.InterpolatedStates.Enabled)
					{
						throw new InvalidOperationException("Cannot disable read buffers unless the interpolated states are disabled.");
					}
					_6S();
					base.Enabled = false;
				}
				else if (!base.Enabled && value)
				{
					_6w();
					base.Enabled = true;
				}
			}
		}

		public object FlipLocker
		{
			[CompilerGenerated]
			get
			{
				return a57;
			}
			[CompilerGenerated]
			private set
			{
				a57 = obj;
			}
		}

		internal void _6w()
		{
			lock (FlipLocker)
			{
				int num = Math.Max(a5h.a5h.Count, 64);
				a5b = new B[num];
				a56 = new B[num];
				for (int i = 0; i < a5h.a5h.Count; i++)
				{
					E.h h2 = a5h.a5h[i];
					a5b[i].Position = h2.a5h;
					a5b[i].Orientation = h2.a5b;
					a5b[i].LinearVelocity = h2.a5a;
					a5b[i].AngularVelocity = h2.a5_0006;
				}
				Array.Copy(a5b, a56, a5b.Length);
			}
		}

		internal void _6S()
		{
			lock (FlipLocker)
			{
				a5b = null;
				a56 = null;
			}
		}

		public a(b manager)
		{
			a5h = manager;
			a5a = _6_0003;
			FlipLocker = new object();
		}

		public a(b manager, d.b threadManager)
		{
			a5h = manager;
			a5a = _6_0003;
			FlipLocker = new object();
			base.ThreadManager = threadManager;
			base.AllowMultithreading = true;
		}

		private void _6_0003(int P_0)
		{
			E.h h2 = a5h.a5h[P_0];
			a5b[P_0].Position = h2.a5h;
			a5b[P_0].Orientation = h2.a5b;
			a5b[P_0].LinearVelocity = h2.a5a;
			a5b[P_0].AngularVelocity = h2.a5_0006;
		}

		protected override void UpdateMultithreaded()
		{
			base.ThreadManager.ForLoop(0, a5h.a5h.Count, a5a);
			FlipBuffers();
		}

		protected override void UpdateSingleThreaded()
		{
			for (int i = 0; i < a5h.a5h.Count; i++)
			{
				E.h h2 = a5h.a5h[i];
				a5b[i].Position = h2.a5h;
				a5b[i].Orientation = h2.a5b;
				a5b[i].LinearVelocity = h2.a5a;
				a5b[i].AngularVelocity = h2.a5_0006;
			}
			FlipBuffers();
		}

		internal void _64(E.h P_0)
		{
			if (a56.Length <= P_0.BufferedStates.a5h)
			{
				B[] array = new B[a56.Length * 2];
				a56.CopyTo(array, 0);
				a56 = array;
			}
			a56[P_0.BufferedStates.a5h].Position = P_0.a5h;
			a56[P_0.BufferedStates.a5h].Orientation = P_0.a5b;
			if (a5b.Length <= P_0.BufferedStates.a5h)
			{
				B[] array2 = new B[a5b.Length * 2];
				a5b.CopyTo(array2, 0);
				a5b = array2;
			}
			a5b[P_0.BufferedStates.a5h].Position = P_0.a5h;
			a5b[P_0.BufferedStates.a5h].Orientation = P_0.a5b;
		}

		internal void _6e(int P_0, int P_1)
		{
			ref B reference = ref a56[P_0];
			reference = a56[P_1];
			ref B reference2 = ref a5b[P_0];
			reference2 = a5b[P_1];
		}

		public void FlipBuffers()
		{
			lock (FlipLocker)
			{
				B[] array = a56;
				a56 = a5b;
				a5b = array;
			}
		}

		public B GetState(int motionStateIndex)
		{
			return a56[motionStateIndex];
		}

		public void GetStates(B[] states)
		{
			lock (FlipLocker)
			{
				if (states.Length < a5h.a5h.Count)
				{
					throw new ArgumentException("Array is not large enough to hold the buffer.", "states");
				}
				Array.Copy(a56, states, a5h.a5h.Count);
			}
		}
	}
}
namespace N
{
	internal struct a(float m11, float m12, float m21, float m22, float m31, float m32)
	{
		public float M11 = m11;

		public float M12 = m12;

		public float M21 = m21;

		public float M22 = m22;

		public float M31 = m31;

		public float M32 = m32;

		public static void Add(ref a a, ref a b, out a result)
		{
			float m = a.M11 + b.M11;
			float m2 = a.M12 + b.M12;
			float m3 = a.M21 + b.M21;
			float m4 = a.M22 + b.M22;
			float m5 = a.M31 + b.M31;
			float m6 = a.M32 + b.M32;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
			result.M31 = m5;
			result.M32 = m6;
		}

		public static void Multiply(ref _7 a, ref a b, out a result)
		{
			float m = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
			float m2 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
			float m3 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
			float m4 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
			float m5 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
			float m6 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
			result.M31 = m5;
			result.M32 = m6;
		}

		public static void Multiply(ref Matrix a, ref a b, out a result)
		{
			float m = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
			float m2 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
			float m3 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
			float m4 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
			float m5 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
			float m6 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
			result.M31 = m5;
			result.M32 = m6;
		}

		public static void Negate(ref a matrix, out a result)
		{
			float m = 0f - matrix.M11;
			float m2 = 0f - matrix.M12;
			float m3 = 0f - matrix.M21;
			float m4 = 0f - matrix.M22;
			float m5 = 0f - matrix.M31;
			float m6 = 0f - matrix.M32;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
			result.M31 = m5;
			result.M32 = m6;
		}

		public static void Subtract(ref a a, ref a b, out a result)
		{
			float m = a.M11 - b.M11;
			float m2 = a.M12 - b.M12;
			float m3 = a.M21 - b.M21;
			float m4 = a.M22 - b.M22;
			float m5 = a.M31 - b.M31;
			float m6 = a.M32 - b.M32;
			result.M11 = m;
			result.M12 = m2;
			result.M21 = m3;
			result.M22 = m4;
			result.M31 = m5;
			result.M32 = m6;
		}

		public static void Transform(ref Vector2 v, ref a matrix, out Vector3 result)
		{
			result = default(Vector3);
			result.X = matrix.M11 * v.X + matrix.M12 * v.Y;
			result.Y = matrix.M21 * v.X + matrix.M22 * v.Y;
			result.Z = matrix.M31 * v.X + matrix.M32 * v.Y;
		}

		public static void Transform(ref Vector3 v, ref a matrix, out Vector2 result)
		{
			result = default(Vector2);
			result.X = v.X * matrix.M11 + v.Y * matrix.M21 + v.Z * matrix.M31;
			result.Y = v.X * matrix.M12 + v.Y * matrix.M22 + v.Z * matrix.M32;
		}

		public static void Transpose(ref a matrix, out _6 result)
		{
			result.M11 = matrix.M11;
			result.M12 = matrix.M21;
			result.M13 = matrix.M31;
			result.M21 = matrix.M12;
			result.M22 = matrix.M22;
			result.M23 = matrix.M32;
		}

		public override string ToString()
		{
			return "{" + M11 + ", " + M12 + "} {" + M21 + ", " + M22 + "} {" + M31 + ", " + M32 + "}";
		}
	}
}
