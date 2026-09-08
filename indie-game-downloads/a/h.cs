using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;

namespace a;

internal class h<T> where T : IWorldBoundingBoxObject
{
	private enum _00065h
	{
		X,
		Y,
		Z,
		None
	}

	private bool a5h;

	private int a5b;

	private BoundingBox a56;

	private _00065h a5a;

	private Plane a57;

	private a.b<T> a5_0006;

	private Dictionary<T, a.h<T>> a5v;

	private List<T> a5B = new List<T>();

	private a.h<T> a5X;

	private a.h<T> a5_0018;

	private static SystemStatistic a5W = SystemConsole.GetStatistic("SceneGraph_CollectObjectsTraversedNodes", SystemStatisticCategory.SceneGraph);

	private static SystemStatistic a5_0002 = SystemConsole.GetStatistic("SceneGraph_CollectObjectsRetrievedObjects", SystemStatisticCategory.SceneGraph);

	private static SystemStatistic a5_000E = SystemConsole.GetStatistic("SceneGraph_CollectObjectsNodeContainTests", SystemStatisticCategory.SceneGraph);

	private static SystemStatistic a5y = SystemConsole.GetStatistic("SceneGraph_CollectObjectsObjectContainTests", SystemStatisticCategory.SceneGraph);

	private static PooledObjectFactory<a.h<T>> a5r = new PooledObjectFactory<a.h<T>>();

	private static float[] a5_0001 = new float[3];

	protected void Init(ref BoundingBox containervolume, int maxdepth, a.b<T> parenttree)
	{
		a5_0006 = parenttree;
		a5v = parenttree.a5h;
		this.T();
		a56 = containervolume;
		a5b = maxdepth;
		Vector3 vector = a56.Max - a56.Min;
		float num = vector.X;
		int num2 = 0;
		a5_0001[0] = vector.X;
		a5_0001[1] = vector.Y;
		a5_0001[2] = vector.Z;
		for (int i = 1; i < 3; i++)
		{
			if (!(num > a5_0001[i]))
			{
				num = a5_0001[i];
				num2 = i;
			}
		}
		a5a = (_00065h)num2;
		a5_0001[0] = 0f;
		a5_0001[1] = 0f;
		a5_0001[2] = 0f;
		a5_0001[num2] = 1f;
		a57.Normal.X = a5_0001[0];
		a57.Normal.Y = a5_0001[1];
		a57.Normal.Z = a5_0001[2];
		a5_0001[0] = a56.Min.X;
		a5_0001[1] = a56.Min.Y;
		a5_0001[2] = a56.Min.Z;
		a57.D = 0f - (a5_0001[num2] + num * 0.5f);
		a5h = a56.Min.X == a5_0006.a56.Min.X || a56.Min.Y == a5_0006.a56.Min.Y || a56.Min.Z == a5_0006.a56.Min.Z || a56.Max.X == a5_0006.a56.Max.X || a56.Max.Y == a5_0006.a56.Max.Y || a56.Max.Z == a5_0006.a56.Max.Z;
	}

	internal virtual void T()
	{
		a5B.Clear();
		if (a5X != null)
		{
			a5X.T();
			a5r.Free(a5X);
			a5X = null;
		}
		if (a5_0018 != null)
		{
			a5_0018.T();
			a5r.Free(a5_0018);
			a5_0018 = null;
		}
	}

	internal void q()
	{
		if (a5X != null)
		{
			a5X.q();
			if (a5X.a5B.Count <= 0 && a5X.a5X == null && a5X.a5_0018 == null)
			{
				a5r.Free(a5X);
				a5X = null;
			}
		}
		if (a5_0018 != null)
		{
			a5_0018.q();
			if (a5_0018.a5B.Count <= 0 && a5_0018.a5X == null && a5_0018.a5_0018 == null)
			{
				a5r.Free(a5_0018);
				a5_0018 = null;
			}
		}
	}

	internal void E(BoundingBox P_0, T P_1)
	{
		if (!a5v.ContainsKey(P_1))
		{
			a.h<T> h2 = Q(ref P_0, 0);
			h2.a5B.Add(P_1);
			a5v.Add(P_1, h2);
		}
	}

	internal void _0013(BoundingBox P_0, T P_1)
	{
		if (a5v.TryGetValue(P_1, out var value))
		{
			a.h<T> h2 = Q(ref P_0, 0);
			if (value != h2)
			{
				value.a5B.Remove(P_1);
				h2.a5B.Add(P_1);
				a5v[P_1] = h2;
			}
		}
	}

	internal void x(BoundingBox P_0, T P_1)
	{
		if (a5v.TryGetValue(P_1, out var value))
		{
			value.a5B.Remove(P_1);
			a5v.Remove(P_1);
		}
	}

	private a.h<T> Q(ref BoundingBox P_0, int P_1)
	{
		if (P_1 == 0)
		{
			a56.Contains(ref P_0, out var result);
			if (result != ContainmentType.Contains)
			{
				return this;
			}
		}
		bool flag = P_0.Max.X * a57.Normal.X + P_0.Max.Y * a57.Normal.Y + P_0.Max.Z * a57.Normal.Z + a57.D > 0f;
		bool flag2 = P_0.Min.X * a57.Normal.X + P_0.Min.Y * a57.Normal.Y + P_0.Min.Z * a57.Normal.Z + a57.D > 0f;
		if (flag != flag2 || P_1 >= a5b)
		{
			return this;
		}
		if (flag2)
		{
			if (a5X == null)
			{
				BoundingBox containervolume = a56;
				containervolume.Min = CoreHelper.ReplaceVectorIndex(containervolume.Min, (int)a5a, 0f - a57.D);
				a5X = a5r.New();
				a5X.Init(ref containervolume, a5b, a5_0006);
			}
			return a5X.Q(ref P_0, P_1 + 1);
		}
		if (a5_0018 == null)
		{
			BoundingBox containervolume2 = a56;
			containervolume2.Max = CoreHelper.ReplaceVectorIndex(containervolume2.Max, (int)a5a, 0f - a57.D);
			a5_0018 = a5r.New();
			a5_0018.Init(ref containervolume2, a5b, a5_0006);
		}
		return a5_0018.Q(ref P_0, P_1 + 1);
	}

	internal void _0012(ref BoundingBox P_0, bool P_1, List<T> P_2)
	{
		a5W.AccumulationValue++;
		ContainmentType result = ContainmentType.Contains;
		if (!P_1)
		{
			a5_000E.AccumulationValue++;
			P_0.Contains(ref a56, out result);
		}
		int count = a5B.Count;
		if (result == ContainmentType.Contains)
		{
			P_1 = true;
			for (int i = 0; i < count; i++)
			{
				P_2.Add(a5B[i]);
			}
			a5_0002.AccumulationValue += count;
		}
		else
		{
			P_1 = false;
			for (int j = 0; j < count; j++)
			{
				T item = a5B[j];
				if (P_0.Contains(item.WorldBoundingBox) != ContainmentType.Disjoint)
				{
					P_2.Add(item);
					a5_0002.AccumulationValue++;
				}
			}
			a5y.AccumulationValue += count;
		}
		if (a5X != null && P_0.Max.X * a57.Normal.X + P_0.Max.Y * a57.Normal.Y + P_0.Max.Z * a57.Normal.Z + a57.D > 0f)
		{
			a5X._0012(ref P_0, P_1, P_2);
		}
		if (a5_0018 != null && P_0.Min.X * a57.Normal.X + P_0.Min.Y * a57.Normal.Y + P_0.Min.Z * a57.Normal.Z + a57.D < 0f)
		{
			a5_0018._0012(ref P_0, P_1, P_2);
		}
	}

	internal void _0012(ref BoundingFrustum P_0, ref BoundingBox P_1, bool P_2, List<T> P_3)
	{
		_0012(ref P_0, ref P_1, true, P_2, P_3);
	}

	private void _0012(ref BoundingFrustum P_0, ref BoundingBox P_1, bool P_2, bool P_3, List<T> P_4)
	{
		a5W.AccumulationValue++;
		ContainmentType result = ContainmentType.Contains;
		if (!P_3)
		{
			a5_000E.AccumulationValue++;
			P_0.Contains(ref a56, out result);
			if (result == ContainmentType.Disjoint && !P_2)
			{
				return;
			}
		}
		int count = a5B.Count;
		if (result == ContainmentType.Contains)
		{
			P_3 = true;
			for (int i = 0; i < count; i++)
			{
				P_4.Add(a5B[i]);
			}
			a5_0002.AccumulationValue += count;
		}
		else
		{
			P_3 = false;
			for (int j = 0; j < count; j++)
			{
				T item = a5B[j];
				if (P_0.Contains(item.WorldBoundingBox) != ContainmentType.Disjoint)
				{
					P_4.Add(item);
					a5_0002.AccumulationValue++;
				}
			}
			a5y.AccumulationValue += count;
		}
		if (a5X != null && P_1.Max.X * a57.Normal.X + P_1.Max.Y * a57.Normal.Y + P_1.Max.Z * a57.Normal.Z + a57.D > 0f)
		{
			a5X._0012(ref P_0, ref P_1, false, P_3, P_4);
		}
		if (a5_0018 != null && P_1.Min.X * a57.Normal.X + P_1.Min.Y * a57.Normal.Y + P_1.Min.Z * a57.Normal.Z + a57.D < 0f)
		{
			a5_0018._0012(ref P_0, ref P_1, false, P_3, P_4);
		}
	}

	internal void g(List<T> P_0)
	{
		foreach (KeyValuePair<T, a.h<T>> item in a5v)
		{
			P_0.Add(item.Key);
		}
	}

	internal bool P()
	{
		int count = a5v.Count;
		if (count <= 50)
		{
			return false;
		}
		int num = s(0);
		float num2 = (float)num / (float)count;
		return num2 > 0.1f;
	}

	private int s(int P_0)
	{
		int num = 0;
		if (P_0 >= a5b)
		{
			if (a5h)
			{
				num = a5B.Count;
			}
		}
		else
		{
			if (a5X != null)
			{
				num += a5X.s(P_0 + 1);
			}
			if (a5_0018 != null)
			{
				num += a5_0018.s(P_0 + 1);
			}
		}
		return num;
	}
}
