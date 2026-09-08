using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Y;

namespace g;

internal class _7 : Y._0006
{
	private readonly h a5h;

	public Y.b BroadPhase => a5h;

	internal _7(h P_0)
	{
		a5h = P_0;
	}

	public void GetEntries(BoundingBox box, IList<Y.a> entries)
	{
		if (a5h.a5h != null)
		{
			a5h.a5h.y61P_00175(ref box, entries);
		}
	}

	public void GetEntries(BoundingFrustum frustum, IList<Y.a> entries)
	{
		if (a5h.a5h != null)
		{
			a5h.a5h.y61P_00175(ref frustum, entries);
		}
	}

	public void GetEntries(BoundingSphere sphere, IList<Y.a> entries)
	{
		if (a5h.a5h != null)
		{
			a5h.a5h.y61P_00175(ref sphere, entries);
		}
	}

	public bool RayCast(Ray ray, float maximumLength, IList<Y.a> entries)
	{
		if (a5h.a5h != null)
		{
			a5h.a5h.y61P_00175(ref ray, maximumLength, entries);
			return entries.Count > 0;
		}
		return false;
	}

	public bool RayCast(Ray ray, IList<Y.a> entries)
	{
		if (a5h.a5h != null)
		{
			a5h.a5h.y61P_00175(ref ray, float.MaxValue, entries);
			return entries.Count > 0;
		}
		return false;
	}
}
