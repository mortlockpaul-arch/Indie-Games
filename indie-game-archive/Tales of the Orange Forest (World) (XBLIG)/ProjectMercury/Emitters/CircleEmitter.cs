#define DEBUG
using Microsoft.Xna.Framework;

namespace ProjectMercury.Emitters;

public class CircleEmitter : Emitter
{
	private float _radius;

	public bool Ring;

	public bool Radiate;

	public float Radius
	{
		get
		{
			return _radius;
		}
		set
		{
			Guard.ArgumentNotFinite("Radius", value);
			Guard.ArgumentLessThan("Radius", value, float.Epsilon);
			_radius = value;
		}
	}

	public override Emitter DeepCopy()
	{
		CircleEmitter circleEmitter = new CircleEmitter();
		circleEmitter.Radius = Radius;
		circleEmitter.Radiate = Radiate;
		circleEmitter.Ring = Ring;
		CircleEmitter circleEmitter2 = circleEmitter;
		CopyBaseFields(circleEmitter2);
		return circleEmitter2;
	}

	protected override void GenerateOffsetAndForce(out Vector2 offset, out Vector2 force)
	{
		Vector2 vector = RandomHelper.NextUnitVector();
		float num = (Ring ? Radius : (Radius * RandomHelper.NextFloat()));
		offset = new Vector2
		{
			X = vector.X * num,
			Y = vector.Y * num
		};
		force = (Radiate ? vector : RandomHelper.NextUnitVector());
	}
}
