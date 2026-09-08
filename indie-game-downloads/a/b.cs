using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;

namespace a;

internal class b<T> : a.h<T> where T : IWorldBoundingBoxObject
{
	internal Dictionary<T, a.h<T>> a5h = new Dictionary<T, a.h<T>>(128);

	internal b(BoundingBox P_0, int P_1)
	{
		Init(ref P_0, P_1, this);
	}

	public b()
	{
	}

	internal void h(ref BoundingBox P_0, int P_1)
	{
		Init(ref P_0, P_1, this);
	}

	internal override void T()
	{
		a5h.Clear();
		base.T();
	}
}
