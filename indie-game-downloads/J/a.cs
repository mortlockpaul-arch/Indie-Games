using System.Collections.Generic;
using System.Runtime.CompilerServices;
using D;
using P;
using Y;
using q;
using s;
using y;

namespace J;

internal static class a
{
	internal static Dictionary<h, _7> a5h;

	[CompilerGenerated]
	private static _6 a5b;

	public static _6 Factories
	{
		[CompilerGenerated]
		get
		{
			return a5b;
		}
		[CompilerGenerated]
		private set
		{
			a5b = obj;
		}
	}

	public static Dictionary<h, _7> CollisionManagers
	{
		get
		{
			return a5h;
		}
		set
		{
			a5h = value;
		}
	}

	static a()
	{
		Factories = new _6();
		a5h = new Dictionary<h, _7>();
		a5h.Add(new h(typeof(s.B<y._6>), typeof(s.B<y._6>)), Factories.BoxBox);
		a5h.Add(new h(typeof(s.B<y._6>), typeof(s.B<y._7>)), Factories.BoxSphere);
		a5h.Add(new h(typeof(s.B<y._7>), typeof(s.B<y._7>)), Factories.SphereSphere);
		a5h.Add(new h(typeof(s.B<y._6>), typeof(s.B<y.v>)), Factories.TriangleConvex);
		a5h.Add(new h(typeof(s.B<y._7>), typeof(s.B<y.v>)), Factories.TriangleConvex);
		a5h.Add(new h(typeof(s.B<y.v>), typeof(s.B<y.v>)), Factories.TriangleConvex);
		a5h.Add(new h(typeof(s.B<y.b>), typeof(s.B<y.v>)), Factories.TriangleConvex);
		a5h.Add(new h(typeof(s.B<y._6>), typeof(P._7)), Factories.StaticMeshConvex);
		a5h.Add(new h(typeof(s.B<y._7>), typeof(P._7)), Factories.StaticMeshSphere);
		a5h.Add(new h(typeof(s.B<y.v>), typeof(P._7)), Factories.StaticMeshConvex);
		a5h.Add(new h(typeof(s.B<y.b>), typeof(P._7)), Factories.StaticMeshConvex);
		a5h.Add(new h(typeof(s.B<y._6>), typeof(P._0006)), Factories.TerrainConvex);
		a5h.Add(new h(typeof(s.B<y._7>), typeof(P._0006)), Factories.TerrainSphere);
		a5h.Add(new h(typeof(s.B<y.v>), typeof(P._0006)), Factories.TerrainConvex);
		a5h.Add(new h(typeof(s.B<y.b>), typeof(P._0006)), Factories.TerrainConvex);
		a5h.Add(new h(typeof(s.B<y._6>), typeof(s._6)), Factories.CompoundConvex);
		a5h.Add(new h(typeof(s.B<y._7>), typeof(s._6)), Factories.CompoundConvex);
		a5h.Add(new h(typeof(s.B<y.v>), typeof(s._6)), Factories.CompoundConvex);
		a5h.Add(new h(typeof(s.B<y.b>), typeof(s._6)), Factories.CompoundConvex);
		a5h.Add(new h(typeof(s._6), typeof(s._6)), Factories.CompoundCompound);
		a5h.Add(new h(typeof(s._6), typeof(P._7)), Factories.CompoundStaticMesh);
		a5h.Add(new h(typeof(s._6), typeof(P._0006)), Factories.CompoundTerrain);
	}

	public static D.h GetPair(ref Y._7 pair)
	{
		if (a5h.TryGetValue(new h(pair.a5h.GetType(), pair.a5b.GetType()), out var value))
		{
			D.h narrowPhasePair = value.GetNarrowPhasePair();
			narrowPhasePair.BroadPhaseOverlap = pair;
			narrowPhasePair.Factory = value;
			return narrowPhasePair;
		}
		s.v v2 = pair.a5h as s.v;
		s.v v3 = pair.a5b as s.v;
		if (v2 != null && v3 != null)
		{
			D.h narrowPhasePair2 = Factories.ConvexConvex.GetNarrowPhasePair();
			narrowPhasePair2.BroadPhaseOverlap = pair;
			narrowPhasePair2.Factory = Factories.ConvexConvex;
			return narrowPhasePair2;
		}
		return null;
	}

	public static D.h GetPair(Y.a entryA, Y.a entryB, q.a rule)
	{
		Y._7 pair = new Y._7(entryA, entryB, rule);
		return GetPair(ref pair);
	}

	public static D.h GetPair(Y.a entryA, Y.a entryB)
	{
		Y._7 pair = new Y._7(entryA, entryB);
		return GetPair(ref pair);
	}

	public static D.b GetPairHandler(ref P._6 pair, q.a rule)
	{
		Y._7 pair2 = new Y._7(pair.a5h, pair.a5b, rule);
		return GetPair(ref pair2) as D.b;
	}

	public static D.b GetPairHandler(ref P._6 pair)
	{
		Y._7 pair2 = new Y._7(pair.a5h, pair.a5b);
		return GetPair(ref pair2) as D.b;
	}

	public static bool Intersecting(ref P._6 pair)
	{
		D.b pairHandler = GetPairHandler(ref pair);
		pairHandler.SuppressEvents = true;
		pairHandler.UpdateCollision(0f);
		bool result = pairHandler.ContactCount > 0;
		pairHandler.SuppressEvents = false;
		pairHandler.CleanUp();
		pairHandler.Factory.GiveBack(pairHandler);
		return result;
	}
}
