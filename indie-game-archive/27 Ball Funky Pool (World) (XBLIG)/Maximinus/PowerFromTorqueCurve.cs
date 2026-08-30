using Microsoft.Xna.Framework;

namespace Maximinus;

public class PowerFromTorqueCurve : CurveRelationship
{
	public PowerFromTorqueCurve(RelationShipCB CB, float precision, Rectangle drawLocation, Color drawColor, Color bgCol)
		: base(CB, precision, drawLocation, drawColor, bgCol)
	{
	}

	public PowerFromTorqueCurve(RelationShipCB CB, float precision)
		: base(CB, precision)
	{
	}

	public override float Evaluate(float x)
	{
		return base.Evaluate(x) * x;
	}
}
