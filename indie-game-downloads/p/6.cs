using System;
using BEPUphysics.DataStructures;
using E;
using Microsoft.Xna.Framework;
using Y;
using Z;
using i;
using l;
using r;
using s;
using y;

namespace P
{
	internal struct _6 : IEquatable<_6>
	{
		internal h a5h;

		internal h a5b;

		public h CollidableA => a5h;

		public h CollidableB => a5b;

		public _6(h collidableA, h collidableB)
		{
			a5h = collidableA;
			a5b = collidableB;
		}

		public override int GetHashCode()
		{
			return a5h.GetHashCode() + a5b.GetHashCode();
		}

		public bool Equals(_6 other)
		{
			if (other.a5h != a5h || other.a5b != a5b)
			{
				if (other.a5h == a5b)
				{
					return other.a5b == a5h;
				}
				return false;
			}
			return true;
		}
	}
}
namespace p
{
	internal static class _6
	{
		private static p.h<l._7<r._0006>> a5h;

		private static p.h<l._7<r._7>> a5b;

		private static p.h<l._7<Y.a>> a56;

		private static p.h<l._7<int>> a5a;

		private static p.h<HashSet<int>> a57;

		private static p.h<l._7<float>> a5_0006;

		private static p.h<l._7<Vector3>> a5v;

		private static p.h<l._7<E.h>> a5B;

		private static p.h<y.v> a5X;

		private static p.h<l._7<s._7>> a5_0018;

		private static p.h<s.X> a5W;

		private static p.h<l._7<i._0006._00065b>> a5_0002;

		private static p.h<Z._7> a5_000E;

		static _6()
		{
			ResetPools();
		}

		public static void ResetPools()
		{
			a5h = new p.b<l._7<r._0006>>();
			a5b = new p.b<l._7<r._7>>();
			a56 = new p.b<l._7<Y.a>>();
			a5_0018 = new p.b<l._7<s._7>>();
			a5a = new p.b<l._7<int>>();
			a57 = new p.b<HashSet<int>>();
			a5_0006 = new p.b<l._7<float>>();
			a5v = new p.b<l._7<Vector3>>();
			a5B = new p.b<l._7<E.h>>(16);
			a5X = new p.b<y.v>();
			a5W = new p.b<s.X>();
			a5_0002 = new p.b<l._7<i._0006._00065b>>();
			a5_000E = new p.b<Z._7>();
		}

		public static l._7<r._7> GetRayCastResultList()
		{
			return a5b.Take();
		}

		public static void GiveBack(l._7<r._7> list)
		{
			list.Clear();
			a5b.GiveBack(list);
		}

		public static l._7<r._0006> GetRayHitList()
		{
			return a5h.Take();
		}

		public static void GiveBack(l._7<r._0006> list)
		{
			list.Clear();
			a5h.GiveBack(list);
		}

		public static l._7<Y.a> GetCollisionEntryList()
		{
			return a56.Take();
		}

		public static void GiveBack(l._7<Y.a> list)
		{
			list.Clear();
			a56.GiveBack(list);
		}

		public static l._7<s._7> GetCompoundChildList()
		{
			return a5_0018.Take();
		}

		public static void GiveBack(l._7<s._7> list)
		{
			list.Clear();
			a5_0018.GiveBack(list);
		}

		public static l._7<int> GetIntList()
		{
			return a5a.Take();
		}

		public static void GiveBack(l._7<int> list)
		{
			list.Clear();
			a5a.GiveBack(list);
		}

		public static HashSet<int> GetIntSet()
		{
			return a57.Take();
		}

		public static void GiveBack(HashSet<int> set)
		{
			set.Clear();
			a57.GiveBack(set);
		}

		public static l._7<float> GetFloatList()
		{
			return a5_0006.Take();
		}

		public static void GiveBack(l._7<float> list)
		{
			list.Clear();
			a5_0006.GiveBack(list);
		}

		public static l._7<Vector3> GetVectorList()
		{
			return a5v.Take();
		}

		public static void GiveBack(l._7<Vector3> list)
		{
			list.Clear();
			a5v.GiveBack(list);
		}

		public static l._7<E.h> GetEntityRawList()
		{
			return a5B.Take();
		}

		public static void GiveBack(l._7<E.h> list)
		{
			list.Clear();
			a5B.GiveBack(list);
		}

		public static y.v GetTriangle(ref Vector3 v1, ref Vector3 v2, ref Vector3 v3)
		{
			y.v v4 = a5X.Take();
			v4.a5h = v1;
			v4.a5b = v2;
			v4.a56 = v3;
			return v4;
		}

		public static y.v GetTriangle()
		{
			return a5X.Take();
		}

		public static void GiveBack(y.v triangle)
		{
			triangle.collisionMargin = 0f;
			triangle.a5a = y._0006.DoubleSided;
			a5X.GiveBack(triangle);
		}

		public static s.X GetTriangleCollidable(ref Vector3 a, ref Vector3 b, ref Vector3 c)
		{
			s.X x2 = a5W.Take();
			y.v shape = x2.Shape;
			shape.a5h = a;
			shape.a5b = b;
			shape.a56 = c;
			return x2;
		}

		public static s.X GetTriangleCollidable()
		{
			return a5W.Take();
		}

		public static void GiveBack(s.X triangle)
		{
			triangle.CleanUp();
			a5W.GiveBack(triangle);
		}

		public static l._7<i._0006._00065b> GetTriangleIndicesList()
		{
			return a5_0002.Take();
		}

		public static void GiveBack(l._7<i._0006._00065b> triangleIndices)
		{
			triangleIndices.Clear();
			a5_0002.GiveBack(triangleIndices);
		}

		public static Z._7 GetSimulationIslandConnection()
		{
			return a5_000E.Take();
		}

		public static void GiveBack(Z._7 connection)
		{
			connection.bV();
			a5_000E.GiveBack(connection);
		}
	}
}
