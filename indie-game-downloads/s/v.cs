using I;
using Microsoft.Xna.Framework;
using N;
using r;
using y;

namespace s;

internal abstract class v : b
{
	public new y.h Shape => (y.h)a5h;

	protected v(y.h P_0)
		: base(P_0)
	{
	}

	public override bool ConvexCast(y.h castShape, ref N._0006 startingTransform, ref Vector3 sweep, out r._0006 hit)
	{
		return I.v.Sweep(castShape, Shape, ref sweep, ref r.X.ZeroVector, ref startingTransform, ref worldTransform, out hit);
	}
}
