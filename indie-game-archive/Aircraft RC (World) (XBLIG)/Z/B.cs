using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;

namespace Z
{
	internal class B<T> where T : IWorldBoundingBoxObject
	{
		private enum _0001CB
		{
			X,
			Y,
			Z,
			None
		}

		private bool HCB;

		private int HC_0002;

		private BoundingBox HC_0012;

		private _0001CB HCH;

		private Plane HC7;

		private global::Z._0002<T> HC_0001;

		private Dictionary<T, global::Z.B<T>> HCw;

		private List<T> HCZ = new List<T>();

		private global::Z.B<T> HC_000F;

		private global::Z.B<T> HCy;

		private static SystemStatistic HC6 = SystemConsole.GetStatistic("SceneGraph_CollectObjectsTraversedNodes", SystemStatisticCategory.SceneGraph);

		private static SystemStatistic HCD = SystemConsole.GetStatistic("SceneGraph_CollectObjectsRetrievedObjects", SystemStatisticCategory.SceneGraph);

		private static SystemStatistic HC_0011 = SystemConsole.GetStatistic("SceneGraph_CollectObjectsNodeContainTests", SystemStatisticCategory.SceneGraph);

		private static SystemStatistic HCK = SystemConsole.GetStatistic("SceneGraph_CollectObjectsObjectContainTests", SystemStatisticCategory.SceneGraph);

		private static PooledObjectFactory<global::Z.B<T>> HC_0003 = new PooledObjectFactory<global::Z.B<T>>();

		private static float[] HCk = new float[3];

		protected void Init(ref BoundingBox containervolume, int maxdepth, global::Z._0002<T> parenttree)
		{
			HC_0001 = parenttree;
			HCw = parenttree.HCB;
			G();
			HC_0012 = containervolume;
			HC_0002 = maxdepth;
			Vector3 vector = HC_0012.Max - HC_0012.Min;
			float num = vector.X;
			int num2 = 0;
			HCk[0] = vector.X;
			HCk[1] = vector.Y;
			HCk[2] = vector.Z;
			for (int i = 1; i < 3; i++)
			{
				if (!(num > HCk[i]))
				{
					num = HCk[i];
					num2 = i;
				}
			}
			HCH = (_0001CB)num2;
			HCk[0] = 0f;
			HCk[1] = 0f;
			HCk[2] = 0f;
			HCk[num2] = 1f;
			HC7.Normal.X = HCk[0];
			HC7.Normal.Y = HCk[1];
			HC7.Normal.Z = HCk[2];
			HCk[0] = HC_0012.Min.X;
			HCk[1] = HC_0012.Min.Y;
			HCk[2] = HC_0012.Min.Z;
			HC7.D = 0f - (HCk[num2] + num * 0.5f);
			HCB = HC_0012.Min.X == HC_0001.HC_0012.Min.X || HC_0012.Min.Y == HC_0001.HC_0012.Min.Y || HC_0012.Min.Z == HC_0001.HC_0012.Min.Z || HC_0012.Max.X == HC_0001.HC_0012.Max.X || HC_0012.Max.Y == HC_0001.HC_0012.Max.Y || HC_0012.Max.Z == HC_0001.HC_0012.Max.Z;
		}

		internal virtual void G()
		{
			HCZ.Clear();
			if (HC_000F != null)
			{
				HC_000F.G();
				HC_0003.Free(HC_000F);
				HC_000F = null;
			}
			if (HCy != null)
			{
				HCy.G();
				HC_0003.Free(HCy);
				HCy = null;
			}
		}

		internal void e()
		{
			if (HC_000F != null)
			{
				HC_000F.e();
				if (HC_000F.HCZ.Count <= 0 && HC_000F.HC_000F == null && HC_000F.HCy == null)
				{
					HC_0003.Free(HC_000F);
					HC_000F = null;
				}
			}
			if (HCy != null)
			{
				HCy.e();
				if (HCy.HCZ.Count <= 0 && HCy.HC_000F == null && HCy.HCy == null)
				{
					HC_0003.Free(HCy);
					HCy = null;
				}
			}
		}

		internal void _0015(BoundingBox P_0, T P_1)
		{
			if (!HCw.ContainsKey(P_1))
			{
				global::Z.B<T> b = j(ref P_0, 0);
				b.HCZ.Add(P_1);
				HCw.Add(P_1, b);
			}
		}

		internal void U(BoundingBox P_0, T P_1)
		{
			if (HCw.TryGetValue(P_1, out var value))
			{
				global::Z.B<T> b = j(ref P_0, 0);
				if (value != b)
				{
					value.HCZ.Remove(P_1);
					b.HCZ.Add(P_1);
					HCw[P_1] = b;
				}
			}
		}

		internal void _8(BoundingBox P_0, T P_1)
		{
			if (HCw.TryGetValue(P_1, out var value))
			{
				value.HCZ.Remove(P_1);
				HCw.Remove(P_1);
			}
		}

		private global::Z.B<T> j(ref BoundingBox P_0, int P_1)
		{
			if (P_1 == 0)
			{
				HC_0012.Contains(ref P_0, out var result);
				if (result != ContainmentType.Contains)
				{
					return this;
				}
			}
			bool flag = P_0.Max.X * HC7.Normal.X + P_0.Max.Y * HC7.Normal.Y + P_0.Max.Z * HC7.Normal.Z + HC7.D > 0f;
			bool flag2 = P_0.Min.X * HC7.Normal.X + P_0.Min.Y * HC7.Normal.Y + P_0.Min.Z * HC7.Normal.Z + HC7.D > 0f;
			if (flag != flag2 || P_1 >= HC_0002)
			{
				return this;
			}
			if (flag2)
			{
				if (HC_000F == null)
				{
					BoundingBox containervolume = HC_0012;
					containervolume.Min = CoreHelper.ReplaceVectorIndex(containervolume.Min, (int)HCH, 0f - HC7.D);
					HC_000F = HC_0003.New();
					HC_000F.Init(ref containervolume, HC_0002, HC_0001);
				}
				return HC_000F.j(ref P_0, P_1 + 1);
			}
			if (HCy == null)
			{
				BoundingBox containervolume2 = HC_0012;
				containervolume2.Max = CoreHelper.ReplaceVectorIndex(containervolume2.Max, (int)HCH, 0f - HC7.D);
				HCy = HC_0003.New();
				HCy.Init(ref containervolume2, HC_0002, HC_0001);
			}
			return HCy.j(ref P_0, P_1 + 1);
		}

		internal void i(ref BoundingBox P_0, bool P_1, List<T> P_2)
		{
			HC6.AccumulationValue++;
			ContainmentType result = ContainmentType.Contains;
			if (!P_1)
			{
				HC_0011.AccumulationValue++;
				P_0.Contains(ref HC_0012, out result);
			}
			int count = HCZ.Count;
			if (result == ContainmentType.Contains)
			{
				P_1 = true;
				for (int i = 0; i < count; i++)
				{
					P_2.Add(HCZ[i]);
				}
				HCD.AccumulationValue += count;
			}
			else
			{
				P_1 = false;
				for (int j = 0; j < count; j++)
				{
					T item = HCZ[j];
					if (P_0.Contains(item.WorldBoundingBox) != ContainmentType.Disjoint)
					{
						P_2.Add(item);
						HCD.AccumulationValue++;
					}
				}
				HCK.AccumulationValue += count;
			}
			if (HC_000F != null && P_0.Max.X * HC7.Normal.X + P_0.Max.Y * HC7.Normal.Y + P_0.Max.Z * HC7.Normal.Z + HC7.D > 0f)
			{
				HC_000F.i(ref P_0, P_1, P_2);
			}
			if (HCy != null && P_0.Min.X * HC7.Normal.X + P_0.Min.Y * HC7.Normal.Y + P_0.Min.Z * HC7.Normal.Z + HC7.D < 0f)
			{
				HCy.i(ref P_0, P_1, P_2);
			}
		}

		internal void i(ref BoundingFrustum P_0, ref BoundingBox P_1, bool P_2, List<T> P_3)
		{
			i(ref P_0, ref P_1, true, P_2, P_3);
		}

		private void i(ref BoundingFrustum P_0, ref BoundingBox P_1, bool P_2, bool P_3, List<T> P_4)
		{
			HC6.AccumulationValue++;
			ContainmentType result = ContainmentType.Contains;
			if (!P_3)
			{
				HC_0011.AccumulationValue++;
				P_0.Contains(ref HC_0012, out result);
				if (result == ContainmentType.Disjoint && !P_2)
				{
					return;
				}
			}
			int count = HCZ.Count;
			if (result == ContainmentType.Contains)
			{
				P_3 = true;
				for (int i = 0; i < count; i++)
				{
					P_4.Add(HCZ[i]);
				}
				HCD.AccumulationValue += count;
			}
			else
			{
				P_3 = false;
				for (int j = 0; j < count; j++)
				{
					T item = HCZ[j];
					if (P_0.Contains(item.WorldBoundingBox) != ContainmentType.Disjoint)
					{
						P_4.Add(item);
						HCD.AccumulationValue++;
					}
				}
				HCK.AccumulationValue += count;
			}
			if (HC_000F != null && P_1.Max.X * HC7.Normal.X + P_1.Max.Y * HC7.Normal.Y + P_1.Max.Z * HC7.Normal.Z + HC7.D > 0f)
			{
				HC_000F.i(ref P_0, ref P_1, false, P_3, P_4);
			}
			if (HCy != null && P_1.Min.X * HC7.Normal.X + P_1.Min.Y * HC7.Normal.Y + P_1.Min.Z * HC7.Normal.Z + HC7.D < 0f)
			{
				HCy.i(ref P_0, ref P_1, false, P_3, P_4);
			}
		}

		internal void O(List<T> P_0)
		{
			foreach (KeyValuePair<T, global::Z.B<T>> item in HCw)
			{
				P_0.Add(item.Key);
			}
		}

		internal bool _0017()
		{
			int count = HCw.Count;
			if (count <= 50)
			{
				return false;
			}
			int num = _0019(0);
			float num2 = (float)num / (float)count;
			return num2 > 0.1f;
		}

		private int _0019(int P_0)
		{
			int num = 0;
			if (P_0 >= HC_0002)
			{
				if (HCB)
				{
					num = HCZ.Count;
				}
			}
			else
			{
				if (HC_000F != null)
				{
					num += HC_000F._0019(P_0 + 1);
				}
				if (HCy != null)
				{
					num += HCy._0019(P_0 + 1);
				}
			}
			return num;
		}
	}
}
namespace z
{
	internal abstract class B : IDisposable
	{
		protected int KeySizeValue;

		protected _000F[] LegalKeySizesValue;

		public abstract string KeyExchangeAlgorithm { get; }

		public virtual int KeySize
		{
			get
			{
				return KeySizeValue;
			}
			set
			{
				if (!_000F.H3(LegalKeySizesValue, value))
				{
					throw new _7("Key size not supported by algorithm.");
				}
				KeySizeValue = value;
			}
		}

		public virtual _000F[] LegalKeySizes => LegalKeySizesValue;

		public abstract string SignatureAlgorithm { get; }

		void IDisposable.Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void Clear()
		{
			Dispose(disposing: false);
		}

		protected abstract void Dispose(bool disposing);

		public static B Create()
		{
			return new D();
		}
	}
}
