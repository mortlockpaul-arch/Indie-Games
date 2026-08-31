using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Rendering;

namespace u;

internal class Z
{
	internal static bool _7w(List<RenderableMesh> P_0, BoundingBox P_1)
	{
		bool flag = false;
		foreach (RenderableMesh item in P_0)
		{
			ISceneObject sceneObject = item.HC_0002;
			item.HCY = P_1.Contains(sceneObject.WorldBoundingBox) != ContainmentType.Disjoint;
			flag |= item.HCY;
		}
		return flag;
	}

	internal static bool _7Z(List<RenderableMesh> P_0, BoundingFrustum P_1)
	{
		bool flag = false;
		foreach (RenderableMesh item in P_0)
		{
			ISceneObject sceneObject = item.HC_0002;
			item.HCY = sceneObject.CastShadows && P_1.Contains(sceneObject.WorldBoundingBox) != ContainmentType.Disjoint;
			flag |= item.HCY;
		}
		return flag;
	}
}
