using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Y;
using l;
using p;

namespace g;

internal sealed class _6 : b
{
	private class _00065h : IComparer<a>
	{
		public int Compare(a x, a y)
		{
			if (!(x.a5h.Min.X < y.a5h.Min.X))
			{
				return 1;
			}
			return -1;
		}
	}

	private class _00065b : IComparer<a>
	{
		public int Compare(a x, a y)
		{
			if (!(x.a5h.Min.Y < y.a5h.Min.Y))
			{
				return 1;
			}
			return -1;
		}
	}

	private class _000656 : IComparer<a>
	{
		public int Compare(a x, a y)
		{
			if (!(x.a5h.Min.Z < y.a5h.Min.Z))
			{
				return 1;
			}
			return -1;
		}
	}

	internal new b a5h;

	internal b a5b;

	internal float a56;

	internal float a5a;

	internal static float a57 = 1.4f;

	internal static p.b<_6> a5_0006 = new p.b<_6>();

	internal static p.b<l._7<a>> a5v = new p.b<l._7<a>>();

	private static _00065h a5B = new _00065h();

	private static _00065b a5X = new _00065b();

	private static _000656 a5_0018 = new _000656();

	internal override b ChildA => a5h;

	internal override b ChildB => a5b;

	internal override Y.a Element => null;

	internal override bool IsLeaf => false;

	internal override void y61P_00175(ref BoundingBox P_0, IList<Y.a> P_1)
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

	internal override void y61P_00175(ref BoundingSphere P_0, IList<Y.a> P_1)
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

	internal override void y61P_00175(ref BoundingFrustum P_0, IList<Y.a> P_1)
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

	internal override void y61P_00175(ref Ray P_0, float P_1, IList<Y.a> P_2)
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

	internal override void y61P_00175(b P_0, h P_1)
	{
		bool result;
		if (this == P_0)
		{
			if (!a5h.IsLeaf)
			{
				a5h.y61P_00175(a5h, P_1);
			}
			if (!a5b.IsLeaf)
			{
				a5b.y61P_00175(a5b, P_1);
			}
			a5h.a5h.Intersects(ref a5b.a5h, out result);
			if (result)
			{
				a5h.y61P_00175(a5b, P_1);
			}
			return;
		}
		if (P_0.IsLeaf)
		{
			a5h.a5h.Intersects(ref P_0.a5h, out result);
			if (result)
			{
				a5h.y61P_00175(P_0, P_1);
			}
			a5b.a5h.Intersects(ref P_0.a5h, out result);
			if (result)
			{
				a5b.y61P_00175(P_0, P_1);
			}
			return;
		}
		b b2 = P_0.ChildA;
		b b3 = P_0.ChildB;
		a5h.a5h.Intersects(ref b2.a5h, out result);
		if (result)
		{
			a5h.y61P_00175(b2, P_1);
		}
		a5h.a5h.Intersects(ref b3.a5h, out result);
		if (result)
		{
			a5h.y61P_00175(b3, P_1);
		}
		a5b.a5h.Intersects(ref b2.a5h, out result);
		if (result)
		{
			a5b.y61P_00175(b2, P_1);
		}
		a5b.a5h.Intersects(ref b3.a5h, out result);
		if (result)
		{
			a5b.y61P_00175(b3, P_1);
		}
	}

	internal override bool _00138_0015_0006f5(a P_0, out b P_1)
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
				_6 obj = a5_0006.Take();
				((b)obj).a5h = result;
				obj.a5h = a5h;
				obj.a5b = P_0;
				obj.a56 = num3;
				a5h = obj;
				P_1 = null;
				return true;
			}
			a5h.a5h = result;
			_6 obj2 = (_6)a5h;
			obj2.a56 = num3;
			P_1 = a5h;
			return false;
		}
		if (a5b.IsLeaf)
		{
			_6 obj3 = a5_0006.Take();
			((b)obj3).a5h = result2;
			obj3.a5h = P_0;
			obj3.a5b = a5b;
			obj3.a56 = num4;
			a5b = obj3;
			P_1 = null;
			return true;
		}
		a5b.a5h = result2;
		P_1 = a5b;
		_6 obj4 = (_6)a5b;
		obj4.a56 = num4;
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

	internal override void bQS_0001D5()
	{
		if (a56 > a5a)
		{
			bF();
			return;
		}
		a5h.bQS_0001D5();
		a5b.bQS_0001D5();
		BoundingBox.CreateMerged(ref a5h.a5h, ref a5b.a5h, out base.a5h);
		a56 = (base.a5h.Max.X - base.a5h.Min.X) * (base.a5h.Max.Y - base.a5h.Min.Y) * (base.a5h.Max.Z - base.a5h.Min.Z);
	}

	internal void bF()
	{
		b b2 = a5h;
		b b3 = a5b;
		a5h = null;
		a5b = null;
		l._7<a> obj = a5v.Take();
		b2._6ZeQq5(obj);
		b3._6ZeQq5(obj);
		for (int i = 0; i < obj.a5h; i++)
		{
			obj.Elements[i].bQS_0001D5();
		}
		b_0005(obj, 0, obj.a5h);
		obj.Clear();
		a5v.GiveBack(obj);
	}

	private void b_0005(l._7<a> P_0, int P_1, int P_2)
	{
		BoundingBox.CreateMerged(ref P_0.Elements[P_1].a5h, ref P_0.Elements[P_1 + 1].a5h, out base.a5h);
		for (int i = P_1 + 2; i < P_2; i++)
		{
			BoundingBox.CreateMerged(ref base.a5h, ref P_0.Elements[i].a5h, out base.a5h);
		}
		Vector3.Subtract(ref base.a5h.Max, ref base.a5h.Min, out var result);
		a56 = result.X * result.Y * result.Z;
		a5a = a56 * a57;
		if (result.X > result.Y && result.X > result.Z)
		{
			Array.Sort(P_0.Elements, P_1, P_2 - P_1, a5B);
		}
		else if (result.Y > result.Z)
		{
			Array.Sort(P_0.Elements, P_1, P_2 - P_1, a5X);
		}
		else
		{
			Array.Sort(P_0.Elements, P_1, P_2 - P_1, a5_0018);
		}
		int num = (P_1 + P_2) / 2;
		if (num - P_1 >= 2)
		{
			_6 obj = a5_0006.Take();
			obj.b_0005(P_0, P_1, num);
			a5h = obj;
		}
		else
		{
			a5h = P_0.Elements[P_1];
		}
		if (P_2 - num >= 2)
		{
			_6 obj2 = a5_0006.Take();
			obj2.b_0005(P_0, num, P_2);
			a5b = obj2;
		}
		else
		{
			a5b = P_0.Elements[num];
		}
	}

	internal override void _6ZeQq5(l._7<a> P_0)
	{
		b b2 = a5h;
		b b3 = a5b;
		a5h = null;
		a5b = null;
		a5_0006.GiveBack(this);
		b2._6ZeQq5(P_0);
		b3._6ZeQq5(P_0);
	}

	internal override void snD_0005v(int P_0, int P_1, l._7<b> P_2)
	{
		if (a56 > a5a)
		{
			bF();
		}
		else if (P_1 == P_0)
		{
			P_2.Add(a5h);
			P_2.Add(a5b);
		}
		else
		{
			a5h.snD_0005v(P_0, P_1 + 1, P_2);
			a5b.snD_0005v(P_0, P_1 + 1, P_2);
		}
	}

	internal override void PDR97(int P_0, int P_1)
	{
		if (P_0 > P_1)
		{
			a5h.PDR97(P_0, P_1 + 1);
			a5b.PDR97(P_0, P_1 + 1);
		}
		BoundingBox.CreateMerged(ref a5h.a5h, ref a5b.a5h, out base.a5h);
		a56 = (base.a5h.Max.X - base.a5h.Min.X) * (base.a5h.Max.Y - base.a5h.Min.Y) * (base.a5h.Max.Z - base.a5h.Min.Z);
	}

	internal override void GPr3r5(b P_0, int P_1, int P_2, h P_3, l._7<h._00065h> P_4)
	{
		bool result;
		if (P_2 == P_1)
		{
			if (this == P_0)
			{
				if (!a5h.IsLeaf)
				{
					P_4.Add(new h._00065h
					{
						a5h = a5h,
						a5b = a5h
					});
				}
				if (!a5b.IsLeaf)
				{
					P_4.Add(new h._00065h
					{
						a5h = a5b,
						a5b = a5b
					});
				}
				a5h.a5h.Intersects(ref a5b.a5h, out result);
				if (result)
				{
					P_4.Add(new h._00065h
					{
						a5h = a5h,
						a5b = a5b
					});
				}
				return;
			}
			if (P_0.IsLeaf)
			{
				a5h.a5h.Intersects(ref P_0.a5h, out result);
				if (result)
				{
					P_4.Add(new h._00065h
					{
						a5h = a5h,
						a5b = P_0
					});
				}
				a5b.a5h.Intersects(ref P_0.a5h, out result);
				if (result)
				{
					P_4.Add(new h._00065h
					{
						a5h = a5b,
						a5b = P_0
					});
				}
				return;
			}
			b b2 = P_0.ChildA;
			b b3 = P_0.ChildB;
			a5h.a5h.Intersects(ref b2.a5h, out result);
			if (result)
			{
				P_4.Add(new h._00065h
				{
					a5h = a5h,
					a5b = b2
				});
			}
			a5h.a5h.Intersects(ref b3.a5h, out result);
			if (result)
			{
				P_4.Add(new h._00065h
				{
					a5h = a5h,
					a5b = b3
				});
			}
			a5b.a5h.Intersects(ref b2.a5h, out result);
			if (result)
			{
				P_4.Add(new h._00065h
				{
					a5h = a5b,
					a5b = b2
				});
			}
			a5b.a5h.Intersects(ref b3.a5h, out result);
			if (result)
			{
				P_4.Add(new h._00065h
				{
					a5h = a5b,
					a5b = b3
				});
			}
		}
		else if (this == P_0)
		{
			if (!a5h.IsLeaf)
			{
				a5h.GPr3r5(a5h, P_1, P_2 + 1, P_3, P_4);
			}
			if (!a5b.IsLeaf)
			{
				a5b.GPr3r5(a5b, P_1, P_2 + 1, P_3, P_4);
			}
			a5h.a5h.Intersects(ref a5b.a5h, out result);
			if (result)
			{
				a5h.GPr3r5(a5b, P_1, P_2 + 1, P_3, P_4);
			}
		}
		else if (P_0.IsLeaf)
		{
			a5h.a5h.Intersects(ref P_0.a5h, out result);
			if (result)
			{
				a5h.GPr3r5(P_0, P_1, P_2 + 1, P_3, P_4);
			}
			a5b.a5h.Intersects(ref P_0.a5h, out result);
			if (result)
			{
				a5b.GPr3r5(P_0, P_1, P_2 + 1, P_3, P_4);
			}
		}
		else
		{
			b b4 = P_0.ChildA;
			b b5 = P_0.ChildB;
			a5h.a5h.Intersects(ref b4.a5h, out result);
			if (result)
			{
				a5h.GPr3r5(b4, P_1, P_2 + 1, P_3, P_4);
			}
			a5h.a5h.Intersects(ref b5.a5h, out result);
			if (result)
			{
				a5h.GPr3r5(b5, P_1, P_2 + 1, P_3, P_4);
			}
			a5b.a5h.Intersects(ref b4.a5h, out result);
			if (result)
			{
				a5b.GPr3r5(b4, P_1, P_2 + 1, P_3, P_4);
			}
			a5b.a5h.Intersects(ref b5.a5h, out result);
			if (result)
			{
				a5b.GPr3r5(b5, P_1, P_2 + 1, P_3, P_4);
			}
		}
	}

	internal override bool _0013e_0003d(Y.a P_0, out a P_1, out b P_2)
	{
		if (a5h._0013e_0003d(P_0, out P_1, out P_2))
		{
			if (a5h.IsLeaf)
			{
				P_2 = a5b;
			}
			else
			{
				a5h = P_2;
				P_2 = this;
			}
			return true;
		}
		if (a5b._0013e_0003d(P_0, out P_1, out P_2))
		{
			if (a5b.IsLeaf)
			{
				P_2 = a5h;
			}
			else
			{
				a5b = P_2;
				P_2 = this;
			}
			return true;
		}
		P_2 = this;
		return false;
	}
}
