using Microsoft.Xna.Framework;

namespace OluXNA;

internal class BoneTarget : Target
{
	public ModelWrapper model;

	public int id;

	public BoneTarget()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		pos = Vector3.Zero;
		selected = 0;
		hp = 1;
		score = 10;
		id = 0;
	}

	public BoneTarget(BoneTarget other)
		: base(other)
	{
		model = other.model;
		id = other.id;
	}

	public override Vector3 absolutePos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Transform(pos, model.GetFirstTransform() * enem.Transformation());
	}
}
