using System.Collections.Generic;
using SynapseGaming.LightingSystem.Rendering;

namespace X;

internal class D : IComparer<RenderableMesh>
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
		return a._3AT - b._3AT;
	}
}
