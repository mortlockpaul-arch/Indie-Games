#define DEBUG
using Microsoft.Xna.Framework;

namespace ProjectMercury.Emitters;

public class ConeEmitter : Emitter
{
	private float _direction;

	private float HalfConeAngle;

	private Vector2 ConeExtents;

	public float Direction
	{
		get
		{
			return _direction;
		}
		set
		{
			_direction = value;
			CalculateConeExtents();
		}
	}

	public float ConeAngle
	{
		get
		{
			return HalfConeAngle + HalfConeAngle;
		}
		set
		{
			Guard.ArgumentNotFinite("ConeAngle", value);
			Guard.ArgumentLessThan("ConeAngle", value, float.Epsilon);
			Guard.ArgumentGreaterThan("ConeAngle", value, 6.283185f);
			HalfConeAngle = value * 0.5f;
			CalculateConeExtents();
		}
	}

	private void CalculateConeExtents()
	{
		ConeExtents = new Vector2
		{
			X = Direction - HalfConeAngle,
			Y = Direction + HalfConeAngle
		};
	}

	public override Emitter DeepCopy()
	{
		ConeEmitter coneEmitter = new ConeEmitter();
		coneEmitter.ConeAngle = ConeAngle;
		coneEmitter.Direction = Direction;
		Emitter emitter = coneEmitter;
		CopyBaseFields(emitter);
		return emitter;
	}

	protected override void GenerateOffsetAndForce(out Vector2 offset, out Vector2 force)
	{
		offset = Vector2.Zero;
		float value = RandomHelper.NextFloat(ConeExtents.X, ConeExtents.Y);
		force = new Vector2
		{
			X = Calculator.Cos(value),
			Y = Calculator.Sin(value)
		};
	}
}
