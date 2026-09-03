using Microsoft.Xna.Framework;

namespace OluXNA;

internal class BoneModelTarget : Target
{
	public ModelWrapper model;

	public int id;

	public int boneName;

	public BoneModelTarget()
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

	public BoneModelTarget(BoneModelTarget other)
		: base(other)
	{
		model = other.model;
		boneName = other.boneName;
		id = other.id;
	}

	public override Vector3 absolutePos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Transform(pos, ((boneName < 0) ? Matrix.Identity : model.transforms[boneName]) * enem.Transformation());
	}
}
