using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;
using Z;

namespace u;

internal class _0002
{
	internal const string HCB = "Entities";

	internal const string HC_0002 = "EntityGroups";

	internal const string HC_0012 = "LightGroups";
}
[Flags]
internal enum _0012
{
	LightingEffect = 1,
	BasicEffect_Lighting = 2,
	BasicEffect_NonLighting = 4,
	MiscEffect = 8
}
internal class _0001 : IComparer<RenderableMesh>
{
	public int Compare(RenderableMesh a, RenderableMesh b)
	{
		if (a == b)
		{
			return 0;
		}
		if (a == null)
		{
			return -1;
		}
		if (b == null)
		{
			return 1;
		}
		return a.HCq - b.HCq;
	}
}
internal class _000F
{
	private static global::Z.y<w> HCB = new global::Z.y<w>();

	internal void _7_000F(Matrix P_0, Matrix P_1, Matrix P_2, Matrix P_3, List<w> P_4, List<RenderableMesh> P_5, _0012 P_6)
	{
		P_4.Clear();
		for (int i = 0; i < P_5.Count; i++)
		{
			RenderableMesh renderableMesh = P_5[i];
			if (renderableMesh != null)
			{
				renderableMesh.HCR = false;
			}
		}
		for (int j = 0; j < P_5.Count; j++)
		{
			RenderableMesh renderableMesh2 = P_5[j];
			if (renderableMesh2 == null || renderableMesh2.HCR)
			{
				continue;
			}
			EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(renderableMesh2.HC6);
			if (effectTypeCaster.LightingEffect != null)
			{
				if ((P_6 & _0012.LightingEffect) == 0)
				{
					continue;
				}
			}
			else if (effectTypeCaster.EffectMatrices == null && (P_6 & _0012.MiscEffect) == 0)
			{
				continue;
			}
			bool flag = false;
			if (effectTypeCaster.RenderableEffect != null)
			{
				effectTypeCaster.RenderableEffect.SetViewAndProjection(P_0, P_1, P_2, P_3);
				flag = true;
			}
			else if (effectTypeCaster.EffectLights != null)
			{
				bool lightingEnabled = effectTypeCaster.EffectLights.LightingEnabled;
				if ((lightingEnabled && (P_6 & _0012.BasicEffect_Lighting) == 0) || (!lightingEnabled && (P_6 & _0012.BasicEffect_NonLighting) == 0))
				{
					continue;
				}
			}
			if (effectTypeCaster.EffectMatrices != null)
			{
				effectTypeCaster.EffectMatrices.View = P_0;
				effectTypeCaster.EffectMatrices.Projection = P_2;
				flag = true;
			}
			w w2 = HCB.New();
			w2.G();
			w2.HC_0001 = renderableMesh2.HC6;
			P_4.Add(w2);
			w2.Objects.Add(renderableMesh2);
			renderableMesh2.HCR = true;
			if (!flag)
			{
				continue;
			}
			for (int k = j + 1; k < P_5.Count; k++)
			{
				RenderableMesh renderableMesh3 = P_5[k];
				if (renderableMesh3 != null && !renderableMesh3.HCR && renderableMesh2.HCV == renderableMesh3.HCV)
				{
					w2.Objects.Add(renderableMesh3);
					renderableMesh3.HCR = true;
				}
			}
		}
	}

	internal void _7y(List<w> P_0, List<RenderableMesh> P_1, bool P_2, bool P_3)
	{
		P_0.Clear();
		for (int i = 0; i < P_1.Count; i++)
		{
			RenderableMesh renderableMesh = P_1[i];
			if (renderableMesh != null)
			{
				renderableMesh.HCR = false;
			}
		}
		for (int j = 0; j < P_1.Count; j++)
		{
			RenderableMesh renderableMesh2 = P_1[j];
			if (renderableMesh2 == null || renderableMesh2.HCR || (P_2 && !renderableMesh2.HCY))
			{
				continue;
			}
			w w2 = HCB.New();
			w2.G();
			w2.HC_0001 = renderableMesh2.HC6;
			w2.HC_0002 = renderableMesh2.HCN;
			w2.HC_0012 = renderableMesh2.HCF;
			w2.HCH = renderableMesh2.HCf;
			w2.HC7 = renderableMesh2.HCG;
			w2.Objects.Add(renderableMesh2);
			P_0.Add(w2);
			renderableMesh2.HCR = true;
			for (int k = j + 1; k < P_1.Count; k++)
			{
				RenderableMesh renderableMesh3 = P_1[k];
				if (renderableMesh3 != null && !renderableMesh3.HCR && (!P_2 || renderableMesh3.HCY) && (renderableMesh2.HCV == renderableMesh3.HCV || (!renderableMesh2.HCF && !renderableMesh3.HCF && !renderableMesh2.HCG && !renderableMesh3.HCG && renderableMesh2.HCN == renderableMesh3.HCN && renderableMesh2.HCf == renderableMesh3.HCf && !renderableMesh2.HC_0010 && !renderableMesh3.HC_0010)))
				{
					w2.Objects.Add(renderableMesh3);
					renderableMesh3.HCR = true;
				}
			}
		}
	}

	internal void G()
	{
		HCB.FreeAllTracked();
	}
}
internal class _0011 : IDisposable
{
	private BoundingBox HCB = default(BoundingBox);

	private int HC_0002;

	private IndexBuffer HC_0012;

	private VertexBuffer HCH;

	internal BoundingBox ObjectBoundingBox => HCB;

	internal int VertexCount => HC_0002;

	internal IndexBuffer IndexBuffer => HC_0012;

	internal VertexBuffer VertexBuffer => HCH;

	internal unsafe void _7u(GraphicsDevice P_0, D[] P_1, ushort[] P_2, int P_3)
	{
		if (HCH == null)
		{
			HCH = new VertexBuffer(P_0, typeof(D), 4096, BufferUsage.None);
			HC_0012 = new IndexBuffer(P_0, typeof(ushort), 6144, BufferUsage.None);
		}
		fixed (D* ptr = P_1)
		{
			fixed (Vector3* max = &HCB.Max)
			{
				fixed (Vector3* min = &HCB.Min)
				{
					int num = P_3 * 4;
					D* ptr2 = ptr;
					min->X = (max->X = ptr2->Position.X);
					min->Y = (max->Y = ptr2->Position.Y);
					max->Z = 1f;
					min->Z = 0f;
					ptr2++;
					for (int i = 1; i < num; i++)
					{
						if (ptr2->Position.X > max->X)
						{
							max->X = ptr2->Position.X;
						}
						else if (ptr2->Position.X < min->X)
						{
							min->X = ptr2->Position.X;
						}
						if (ptr2->Position.Y > max->Y)
						{
							max->Y = ptr2->Position.Y;
						}
						else if (ptr2->Position.Y < min->Y)
						{
							min->Y = ptr2->Position.Y;
						}
						ptr2++;
					}
				}
			}
		}
		P_0.Indices = null;
		P_0.SetVertexBuffer(null);
		HC_0002 = P_3 * 4;
		HCH.SetData(P_1, 0, HC_0002);
		HC_0012.SetData(P_2, 0, P_3 * 6);
		P_3 = 0;
	}

	public void Dispose()
	{
		if (HC_0012 != null)
		{
			HC_0012.Dispose();
			HC_0012 = null;
		}
		if (HCH != null)
		{
			HCH.Dispose();
			HCH = null;
		}
	}
}
