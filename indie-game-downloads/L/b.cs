using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace L
{
	internal class b
	{
		internal int a5h;

		internal int a5b = int.MaxValue;

		internal int a56 = DefaultMinimumIterations;

		internal float a5a = DefaultMinimumImpulse;

		internal int a57;

		public static float DefaultMinimumImpulse = 0.001f;

		public static int DefaultMinimumIterations = 1;

		public int MaximumIterations
		{
			get
			{
				return a5b;
			}
			set
			{
				a5b = value;
			}
		}

		public int MinimumIterations
		{
			get
			{
				return a56;
			}
			set
			{
				a56 = value;
			}
		}

		public float MinimumImpulse
		{
			get
			{
				return a5a;
			}
			set
			{
				a5a = value;
			}
		}
	}
}
namespace l
{
	internal class b
	{
		private abstract class _00065h
		{
			internal BoundingBox a5h;

			internal abstract bool IsLeaf { get; }

			internal abstract void y61P_00175(ref BoundingBox P_0, IList<int> P_1);

			internal abstract void y61P_00175(ref BoundingSphere P_0, IList<int> P_1);

			internal abstract void y61P_00175(ref BoundingFrustum P_0, IList<int> P_1);

			internal abstract void y61P_00175(ref Ray P_0, float P_1, IList<int> P_2);

			internal abstract bool _00138_0015_0006f5(_000656 P_0, out _00065h P_1);

			internal abstract void _0017_0017HY1(List<int> P_0, int P_1, ref int P_2);

			internal abstract void bQS_0001D5(_6 P_0);
		}

		private sealed class _00065b : _00065h
		{
			internal new _00065h a5h;

			internal _00065h a5b;

			internal override bool IsLeaf => false;

			internal override void y61P_00175(ref BoundingBox P_0, IList<int> P_1)
			{
				a5h.a5h.Intersects(ref P_0, out var result);
				if (result)
				{
					a5h.y61P_00175(ref P_0, P_1);
				}
				a5b.a5h.Intersects(ref P_0, out result);
				if (result)
				{
					a5b.y61P_00175(ref P_0, P_1);
				}
			}

			internal override void y61P_00175(ref BoundingSphere P_0, IList<int> P_1)
			{
				a5h.a5h.Intersects(ref P_0, out var result);
				if (result)
				{
					a5h.y61P_00175(ref P_0, P_1);
				}
				a5b.a5h.Intersects(ref P_0, out result);
				if (result)
				{
					a5b.y61P_00175(ref P_0, P_1);
				}
			}

			internal override void y61P_00175(ref BoundingFrustum P_0, IList<int> P_1)
			{
				P_0.Intersects(ref a5h.a5h, out var result);
				if (result)
				{
					a5h.y61P_00175(ref P_0, P_1);
				}
				P_0.Intersects(ref a5b.a5h, out result);
				if (result)
				{
					a5b.y61P_00175(ref P_0, P_1);
				}
			}

			internal override void y61P_00175(ref Ray P_0, float P_1, IList<int> P_2)
			{
				P_0.Intersects(ref a5h.a5h, out var result);
				if (result.HasValue && result < P_1)
				{
					a5h.y61P_00175(ref P_0, P_1, P_2);
				}
				P_0.Intersects(ref a5b.a5h, out result);
				if (result.HasValue && result < P_1)
				{
					a5b.y61P_00175(ref P_0, P_1, P_2);
				}
			}

			internal override bool _00138_0015_0006f5(_000656 P_0, out _00065h P_1)
			{
				BoundingBox.CreateMerged(ref a5h.a5h, ref P_0.a5h, out var result);
				BoundingBox.CreateMerged(ref a5b.a5h, ref P_0.a5h, out var result2);
				Vector3.Subtract(ref a5h.a5h.Max, ref a5h.a5h.Min, out var result3);
				float num = result3.X * result3.Y * result3.Z;
				Vector3.Subtract(ref a5b.a5h.Max, ref a5b.a5h.Min, out result3);
				float num2 = result3.X * result3.Y * result3.Z;
				Vector3.Subtract(ref result.Max, ref result.Min, out result3);
				float num3 = result3.X * result3.Y * result3.Z;
				Vector3.Subtract(ref result2.Max, ref result2.Min, out result3);
				float num4 = result3.X * result3.Y * result3.Z;
				if (num3 - num < num4 - num2)
				{
					if (a5h.IsLeaf)
					{
						a5h = new _00065b
						{
							a5h = result,
							a5h = a5h,
							a5b = P_0
						};
						P_1 = null;
						return true;
					}
					a5h.a5h = result;
					P_1 = a5h;
					return false;
				}
				if (a5b.IsLeaf)
				{
					a5b = new _00065b
					{
						a5h = result2,
						a5h = P_0,
						a5b = a5b
					};
					P_1 = null;
					return true;
				}
				a5b.a5h = result2;
				P_1 = a5b;
				return false;
			}

			public override string ToString()
			{
				return "{" + a5h.ToString() + ", " + a5b.ToString() + "}";
			}

			internal override void _0017_0017HY1(List<int> P_0, int P_1, ref int P_2)
			{
				P_2++;
				a5h._0017_0017HY1(P_0, P_1 + 1, ref P_2);
				a5b._0017_0017HY1(P_0, P_1 + 1, ref P_2);
			}

			internal override void bQS_0001D5(_6 P_0)
			{
				a5h.bQS_0001D5(P_0);
				a5b.bQS_0001D5(P_0);
				BoundingBox.CreateMerged(ref a5h.a5h, ref a5b.a5h, out base.a5h);
			}
		}

		private sealed class _000656 : _00065h
		{
			private new int a5h;

			internal override bool IsLeaf => true;

			internal _000656(int P_0, _6 P_1)
			{
				a5h = P_0;
				P_1.GetBoundingBox(P_0, out base.a5h);
				base.a5h.Max.X += LeafMargin;
				base.a5h.Max.Y += LeafMargin;
				base.a5h.Max.Z += LeafMargin;
				base.a5h.Min.X -= LeafMargin;
				base.a5h.Min.Y -= LeafMargin;
				base.a5h.Min.Z -= LeafMargin;
			}

			internal override void y61P_00175(ref BoundingBox P_0, IList<int> P_1)
			{
				P_1.Add(a5h);
			}

			internal override void y61P_00175(ref BoundingSphere P_0, IList<int> P_1)
			{
				P_1.Add(a5h);
			}

			internal override void y61P_00175(ref BoundingFrustum P_0, IList<int> P_1)
			{
				P_1.Add(a5h);
			}

			internal override void y61P_00175(ref Ray P_0, float P_1, IList<int> P_2)
			{
				P_2.Add(a5h);
			}

			internal override bool _00138_0015_0006f5(_000656 P_0, out _00065h P_1)
			{
				_00065b obj = new _00065b();
				BoundingBox.CreateMerged(ref base.a5h, ref ((_00065h)P_0).a5h, out ((_00065h)obj).a5h);
				obj.a5h = this;
				obj.a5b = P_0;
				P_1 = obj;
				return true;
			}

			public override string ToString()
			{
				return a5h.ToString();
			}

			internal override void _0017_0017HY1(List<int> P_0, int P_1, ref int P_2)
			{
				P_2++;
				P_0.Add(P_1);
			}

			internal override void bQS_0001D5(_6 P_0)
			{
				P_0.GetBoundingBox(a5h, out base.a5h);
				base.a5h.Max.X += LeafMargin;
				base.a5h.Max.Y += LeafMargin;
				base.a5h.Max.Z += LeafMargin;
				base.a5h.Min.X -= LeafMargin;
				base.a5h.Min.Y -= LeafMargin;
				base.a5h.Min.Z -= LeafMargin;
			}
		}

		private _6 a5h;

		private _00065h a5b;

		public static float LeafMargin = 0.001f;

		public BoundingBox BoundingBox
		{
			get
			{
				if (a5b != null)
				{
					return a5b.a5h;
				}
				return default(BoundingBox);
			}
		}

		public _6 Data
		{
			get
			{
				return a5h;
			}
			set
			{
				a5h = value;
				Reconstruct();
			}
		}

		public b(_6 data)
		{
			Data = data;
		}

		public void Reconstruct()
		{
			a5b = null;
			for (int i = 0; i < a5h.a5h.Length; i += 3)
			{
				_6x((int)(1208299L * (long)(i / 3) % (a5h.a5h.Length / 3) * 3));
			}
		}

		public void Refit()
		{
			if (a5b != null)
			{
				a5b.bQS_0001D5(a5h);
			}
		}

		private void bt(out List<int> P_0, out int P_1, out int P_2, out int P_3)
		{
			P_0 = new List<int>();
			P_3 = 0;
			a5b._0017_0017HY1(P_0, 0, ref P_3);
			P_2 = 0;
			P_1 = int.MaxValue;
			for (int i = 0; i < P_0.Count; i++)
			{
				if (P_0[i] > P_2)
				{
					P_2 = P_0[i];
				}
				if (P_0[i] < P_1)
				{
					P_1 = P_0[i];
				}
			}
		}

		private void _6x(int P_0)
		{
			_000656 obj = new _000656(P_0, a5h);
			if (a5b == null)
			{
				a5b = obj;
				return;
			}
			if (a5b.IsLeaf)
			{
				a5b._00138_0015_0006f5(obj, out a5b);
				return;
			}
			BoundingBox.CreateMerged(ref obj.a5h, ref a5b.a5h, out a5b.a5h);
			_00065h obj2 = a5b;
			while (!obj2._00138_0015_0006f5(obj, out obj2))
			{
			}
		}

		public bool GetOverlaps(BoundingBox boundingBox, IList<int> outputOverlappedElements)
		{
			if (a5b != null)
			{
				a5b.a5h.Intersects(ref boundingBox, out var result);
				if (result)
				{
					a5b.y61P_00175(ref boundingBox, outputOverlappedElements);
				}
			}
			return outputOverlappedElements.Count > 0;
		}

		public bool GetOverlaps(BoundingSphere boundingSphere, IList<int> outputOverlappedElements)
		{
			if (a5b != null)
			{
				a5b.a5h.Intersects(ref boundingSphere, out var result);
				if (result)
				{
					a5b.y61P_00175(ref boundingSphere, outputOverlappedElements);
				}
			}
			return outputOverlappedElements.Count > 0;
		}

		public bool GetOverlaps(BoundingFrustum boundingFrustum, IList<int> outputOverlappedElements)
		{
			if (a5b != null)
			{
				boundingFrustum.Intersects(ref a5b.a5h, out var result);
				if (result)
				{
					a5b.y61P_00175(ref boundingFrustum, outputOverlappedElements);
				}
			}
			return outputOverlappedElements.Count > 0;
		}

		public bool GetOverlaps(Ray ray, IList<int> outputOverlappedElements)
		{
			if (a5b != null)
			{
				ray.Intersects(ref a5b.a5h, out var result);
				if (result.HasValue)
				{
					a5b.y61P_00175(ref ray, float.MaxValue, outputOverlappedElements);
				}
			}
			return outputOverlappedElements.Count > 0;
		}

		public bool GetOverlaps(Ray ray, float maximumLength, IList<int> outputOverlappedElements)
		{
			if (a5b != null)
			{
				ray.Intersects(ref a5b.a5h, out var result);
				if (result.HasValue)
				{
					a5b.y61P_00175(ref ray, maximumLength, outputOverlappedElements);
				}
			}
			return outputOverlappedElements.Count > 0;
		}
	}
	internal struct B<T>(IEnumerable<T> enumerable) : IEnumerable<T>, IEnumerable
	{
		private readonly IEnumerable<T> a5h = enumerable;

		public IEnumerator<T> GetEnumerator()
		{
			return a5h.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return a5h.GetEnumerator();
		}
	}
}
