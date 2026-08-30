#define DEBUG
using Microsoft.Xna.Framework;

namespace ProjectMercury.Emitters;

public class LineEmitter : Emitter
{
	private float HalfLength;

	private float AngleCos = Calculator.Cos(0f);

	private float AngleSin = Calculator.Sin(0f);

	public bool Rectilinear;

	public bool EmitBothWays;

	public float Length
	{
		get
		{
			return HalfLength + HalfLength;
		}
		set
		{
			Guard.ArgumentNotFinite("Length", value);
			Guard.ArgumentLessThan("Length", value, 0f);
			HalfLength = value * 0.5f;
		}
	}

	public float Angle
	{
		get
		{
			return Calculator.Atan2(AngleSin, AngleCos);
		}
		set
		{
			Guard.ArgumentNotFinite("Angle", Angle);
			AngleCos = Calculator.Cos(value);
			AngleSin = Calculator.Sin(value);
		}
	}

	public override Emitter DeepCopy()
	{
		LineEmitter lineEmitter = new LineEmitter();
		lineEmitter.Angle = Angle;
		lineEmitter.Length = Length;
		lineEmitter.Rectilinear = Rectilinear;
		lineEmitter.EmitBothWays = EmitBothWays;
		Emitter emitter = lineEmitter;
		CopyBaseFields(emitter);
		return emitter;
	}

	protected override void GenerateOffsetAndForce(out Vector2 offset, out Vector2 force)
	{
		float num = RandomHelper.NextFloat(0f - HalfLength, HalfLength);
		offset = new Vector2
		{
			X = num * AngleCos,
			Y = num * AngleSin
		};
		if (Rectilinear)
		{
			force = new Vector2
			{
				X = AngleSin,
				Y = 0f - AngleCos
			};
			if (EmitBothWays && RandomHelper.NextBool())
			{
				force.X *= -1f;
				force.Y *= -1f;
			}
		}
		else
		{
			force = RandomHelper.NextUnitVector();
		}
	}
}
