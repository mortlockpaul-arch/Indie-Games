using System.Collections.Generic;
using _0003;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace X;

internal class L
{
	internal const string _3A_0018 = "Entities";

	internal const string _3AL = "EntityGroups";

	internal const string _3A_0019 = "LightGroups";
}
internal class l
{
	private static global::_0003.t<_0017> _3A_0018 = new global::_0003.t<_0017>();

	internal void _6l(Matrix P_0, Matrix P_1, Matrix P_2, Matrix P_3, List<_0017> P_4, List<RenderableMesh> P_5, _0019 P_6)
	{
		P_4.Clear();
		for (int i = 0; i < P_5.Count; i++)
		{
			RenderableMesh renderableMesh = P_5[i];
			if (renderableMesh != null)
			{
				renderableMesh._3Ay = false;
			}
		}
		for (int j = 0; j < P_5.Count; j++)
		{
			RenderableMesh renderableMesh2 = P_5[j];
			if (renderableMesh2 == null || renderableMesh2._3Ay)
			{
				continue;
			}
			EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(renderableMesh2._3AF);
			if (effectTypeCaster.LightingEffect != null)
			{
				if ((P_6 & _0019.LightingEffect) == 0)
				{
					continue;
				}
			}
			else if (effectTypeCaster.EffectMatrices == null && (P_6 & _0019.MiscEffect) == 0)
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
				if ((lightingEnabled && (P_6 & _0019.BasicEffect_Lighting) == 0) || (!lightingEnabled && (P_6 & _0019.BasicEffect_NonLighting) == 0))
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
			_0017 obj = _3A_0018.New();
			obj.U();
			obj._3AD = renderableMesh2._3AF;
			P_4.Add(obj);
			obj.Objects.Add(renderableMesh2);
			renderableMesh2._3Ay = true;
			if (!flag)
			{
				continue;
			}
			for (int k = j + 1; k < P_5.Count; k++)
			{
				RenderableMesh renderableMesh3 = P_5[k];
				if (renderableMesh3 != null && !renderableMesh3._3Ay && renderableMesh2._3Aq == renderableMesh3._3Aq)
				{
					obj.Objects.Add(renderableMesh3);
					renderableMesh3._3Ay = true;
				}
			}
		}
	}

	internal void _6t(List<_0017> P_0, List<RenderableMesh> P_1, bool P_2, bool P_3)
	{
		P_0.Clear();
		for (int i = 0; i < P_1.Count; i++)
		{
			RenderableMesh renderableMesh = P_1[i];
			if (renderableMesh != null)
			{
				renderableMesh._3Ay = false;
			}
		}
		for (int j = 0; j < P_1.Count; j++)
		{
			RenderableMesh renderableMesh2 = P_1[j];
			if (renderableMesh2 == null || renderableMesh2._3Ay || (P_2 && !renderableMesh2._3Ax))
			{
				continue;
			}
			_0017 obj = _3A_0018.New();
			obj.U();
			obj._3AD = renderableMesh2._3AF;
			obj._3AL = renderableMesh2._3A_0015;
			obj._3A_0019 = renderableMesh2._3A_0001;
			obj._3A3 = renderableMesh2._3A7;
			obj._3A6 = renderableMesh2._3AX;
			obj.Objects.Add(renderableMesh2);
			P_0.Add(obj);
			renderableMesh2._3Ay = true;
			for (int k = j + 1; k < P_1.Count; k++)
			{
				RenderableMesh renderableMesh3 = P_1[k];
				if (renderableMesh3 != null && !renderableMesh3._3Ay && (!P_2 || renderableMesh3._3Ax) && (renderableMesh2._3Aq == renderableMesh3._3Aq || (!renderableMesh2._3A_0001 && !renderableMesh3._3A_0001 && !renderableMesh2._3AX && !renderableMesh3._3AX && renderableMesh2._3A_0015 == renderableMesh3._3A_0015 && renderableMesh2._3A7 == renderableMesh3._3A7 && !renderableMesh2._3A_0010 && !renderableMesh3._3A_0010)))
				{
					obj.Objects.Add(renderableMesh3);
					renderableMesh3._3Ay = true;
				}
			}
		}
	}

	internal void U()
	{
		_3A_0018.FreeAllTracked();
	}
}
