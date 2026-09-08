using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Y;
using l;

namespace g;

internal sealed class a : b
{
	private new Y.a a5h;

	internal override b ChildA => null;

	internal override b ChildB => null;

	internal override Y.a Element => a5h;

	internal override bool IsLeaf => true;

	internal void _4(Y.a P_0)
	{
		a5h = P_0;
		base.a5h = P_0.BoundingBox;
	}

	internal void bV()
	{
		a5h = null;
	}

	internal override void y61P_00175(ref BoundingBox P_0, IList<Y.a> P_1)
	{
		P_1.Add(a5h);
	}

	internal override void y61P_00175(ref BoundingSphere P_0, IList<Y.a> P_1)
	{
		P_1.Add(a5h);
	}

	internal override void y61P_00175(ref BoundingFrustum P_0, IList<Y.a> P_1)
	{
		P_1.Add(a5h);
	}

	internal override void y61P_00175(ref Ray P_0, float P_1, IList<Y.a> P_2)
	{
		P_2.Add(a5h);
	}

	internal override void y61P_00175(b P_0, h P_1)
	{
		if (P_0.IsLeaf)
		{
			P_1.TryToAddOverlap(a5h, P_0.Element);
			return;
		}
		b b2 = P_0.ChildA;
		b b3 = P_0.ChildB;
		base.a5h.Intersects(ref b2.a5h, out var result);
		if (result)
		{
			y61P_00175(b2, P_1);
		}
		base.a5h.Intersects(ref b3.a5h, out result);
		if (result)
		{
			y61P_00175(b3, P_1);
		}
	}

	internal override bool _00138_0015_0006f5(a P_0, out b P_1)
	{
		_6 obj = _6.a5_0006.Take();
		BoundingBox.CreateMerged(ref base.a5h, ref ((b)P_0).a5h, out ((b)obj).a5h);
		Vector3.Subtract(ref ((b)obj).a5h.Max, ref ((b)obj).a5h.Min, out var result);
		obj.a56 = result.X * result.Y * result.Z;
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

	internal override void bQS_0001D5()
	{
		base.a5h = a5h.boundingBox;
	}

	internal override void _6ZeQq5(l._7<a> P_0)
	{
		bQS_0001D5();
		P_0.Add(this);
	}

	internal override void snD_0005v(int P_0, int P_1, l._7<b> P_2)
	{
	}

	internal override void PDR97(int P_0, int P_1)
	{
		base.a5h = a5h.boundingBox;
	}

	internal override void GPr3r5(b P_0, int P_1, int P_2, h P_3, l._7<h._00065h> P_4)
	{
		if (P_0.IsLeaf)
		{
			P_3.TryToAddOverlap(a5h, P_0.Element);
			return;
		}
		b b2 = P_0.ChildA;
		b b3 = P_0.ChildB;
		bool result;
		if (P_1 == P_2)
		{
			base.a5h.Intersects(ref b2.a5h, out result);
			if (result)
			{
				P_4.Add(new h._00065h
				{
					a5h = this,
					a5b = b2
				});
			}
			base.a5h.Intersects(ref b3.a5h, out result);
			if (result)
			{
				P_4.Add(new h._00065h
				{
					a5h = this,
					a5b = b3
				});
			}
		}
		else
		{
			base.a5h.Intersects(ref b2.a5h, out result);
			if (result)
			{
				y61P_00175(b2, P_3);
			}
			base.a5h.Intersects(ref b3.a5h, out result);
			if (result)
			{
				y61P_00175(b3, P_3);
			}
		}
	}

	internal override bool _0013e_0003d(Y.a P_0, out a P_1, out b P_2)
	{
		P_2 = null;
		if (a5h == P_0)
		{
			P_1 = this;
			return true;
		}
		P_1 = null;
		return false;
	}
}
