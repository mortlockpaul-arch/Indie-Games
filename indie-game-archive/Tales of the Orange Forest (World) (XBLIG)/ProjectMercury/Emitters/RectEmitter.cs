#define DEBUG
using Microsoft.Xna.Framework;

namespace ProjectMercury.Emitters;

public class RectEmitter : Emitter
{
	private float HalfWidth;

	private float HalfHeight;

	private float AngleCos = Calculator.Cos(0f);

	private float AngleSin = Calculator.Sin(0f);

	public bool Frame;

	public float Width
	{
		get
		{
			return HalfWidth + HalfWidth;
		}
		set
		{
			Guard.ArgumentNotFinite("Width", value);
			Guard.ArgumentLessThan("Width", value, 0f);
			HalfWidth = value * 0.5f;
		}
	}

	public float Height
	{
		get
		{
			return HalfHeight + HalfHeight;
		}
		set
		{
			Guard.ArgumentNotFinite("Height", value);
			Guard.ArgumentLessThan("Height", value, 0f);
			HalfHeight = value * 0.5f;
		}
	}

	public float Rotation
	{
		get
		{
			return Calculator.Atan2(AngleSin, AngleCos);
		}
		set
		{
			Guard.ArgumentNotFinite("Rotation", value);
			AngleCos = Calculator.Cos(value);
			AngleSin = Calculator.Sin(value);
		}
	}

	public override Emitter DeepCopy()
	{
		RectEmitter rectEmitter = new RectEmitter();
		rectEmitter.Frame = Frame;
		rectEmitter.Height = Height;
		rectEmitter.Rotation = Rotation;
		rectEmitter.Width = Width;
		Emitter emitter = rectEmitter;
		CopyBaseFields(emitter);
		return emitter;
	}

	protected override void GenerateOffsetAndForce(out Vector2 offset, out Vector2 force)
	{
		offset = default(Vector2);
		if (Frame)
		{
			if (RandomHelper.NextBool())
			{
				offset.X = RandomHelper.ChooseOne(0f - HalfWidth, HalfWidth);
				offset.Y = RandomHelper.NextFloat(0f - HalfHeight, HalfHeight);
			}
			else
			{
				offset.X = RandomHelper.NextFloat(0f - HalfWidth, HalfWidth);
				offset.Y = RandomHelper.ChooseOne(0f - HalfHeight, HalfHeight);
			}
		}
		else
		{
			offset.X = RandomHelper.NextFloat(0f - HalfWidth, HalfWidth);
			offset.Y = RandomHelper.NextFloat(0f - HalfHeight, HalfHeight);
		}
		Vector2 vector = offset;
		offset.X = vector.X * AngleCos + vector.Y * (0f - AngleSin);
		offset.Y = vector.X * AngleSin + vector.Y * AngleCos;
		force = RandomHelper.NextUnitVector();
	}
}
