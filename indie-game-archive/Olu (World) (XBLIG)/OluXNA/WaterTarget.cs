using Microsoft.Xna.Framework;

namespace OluXNA;

internal class WaterTarget : Target
{
	public float waterHeight;

	public WaterTarget()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		pos = Vector3.Zero;
		selected = 0;
		hp = 1;
		score = 10;
	}

	public WaterTarget(WaterTarget other)
		: base(other)
	{
		waterHeight = other.waterHeight;
	}

	public override Vector3 absolutePos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = Vector3.Transform(pos, enem.Transformation());
		if (result.Y < waterHeight)
		{
			((Vector3)(ref result))._002Ector(1000f, 1000f, -1000f);
		}
		return result;
	}

	public override bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3.Transform(pos, enem.Transformation()).Y > waterHeight)
		{
			return true;
		}
		return false;
	}
}
