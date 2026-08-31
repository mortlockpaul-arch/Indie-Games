using System.Collections.Generic;
using SynapseGaming.LightingSystem.Rendering;

namespace u;

internal class _7 : IComparer<RenderableMesh>
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
		return a.HCu - b.HCu;
	}
}
