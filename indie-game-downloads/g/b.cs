using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Y;
using l;

namespace g;

internal abstract class b
{
	internal BoundingBox a5h;

	internal abstract bool IsLeaf { get; }

	internal abstract b ChildA { get; }

	internal abstract b ChildB { get; }

	internal abstract Y.a Element { get; }

	internal abstract void y61P_00175(ref BoundingBox P_0, IList<Y.a> P_1);

	internal abstract void y61P_00175(ref BoundingSphere P_0, IList<Y.a> P_1);

	internal abstract void y61P_00175(ref BoundingFrustum P_0, IList<Y.a> P_1);

	internal abstract void y61P_00175(ref Ray P_0, float P_1, IList<Y.a> P_2);

	internal abstract void y61P_00175(b P_0, h P_1);

	internal abstract bool _00138_0015_0006f5(a P_0, out b P_1);

	internal abstract void _0017_0017HY1(List<int> P_0, int P_1, ref int P_2);

	internal abstract void bQS_0001D5();

	internal abstract void _6ZeQq5(l._7<a> P_0);

	internal abstract void snD_0005v(int P_0, int P_1, l._7<b> P_2);

	internal abstract void PDR97(int P_0, int P_1);

	internal abstract void GPr3r5(b P_0, int P_1, int P_2, h P_3, l._7<h._00065h> P_4);

	internal abstract bool _0013e_0003d(Y.a P_0, out a P_1, out b P_2);
}
