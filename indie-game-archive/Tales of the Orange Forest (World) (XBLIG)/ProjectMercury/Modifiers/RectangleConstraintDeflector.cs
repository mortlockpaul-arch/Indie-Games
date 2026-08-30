#define DEBUG
using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public sealed class RectangleConstraintDeflector : Modifier
{
	public Vector2 Position;

	private float _width;

	private float _height;

	private VariableFloat _restitutionCoefficient;

	public float Width
	{
		get
		{
			return _width;
		}
		set
		{
			Guard.ArgumentNotFinite("Width", value);
			Guard.ArgumentLessThan("Width", value, 0f);
			_width = value;
		}
	}

	public float Height
	{
		get
		{
			return _height;
		}
		set
		{
			Guard.ArgumentNotFinite("Height", value);
			Guard.ArgumentLessThan("Height", value, 0f);
			_height = value;
		}
	}

	public VariableFloat RestitutionCoefficient
	{
		get
		{
			return _restitutionCoefficient;
		}
		set
		{
			_restitutionCoefficient = value;
		}
	}

	public override Modifier DeepCopy()
	{
		RectangleConstraintDeflector rectangleConstraintDeflector = new RectangleConstraintDeflector();
		rectangleConstraintDeflector.Height = Height;
		rectangleConstraintDeflector.Position = Position;
		rectangleConstraintDeflector.RestitutionCoefficient = RestitutionCoefficient;
		rectangleConstraintDeflector.Width = Width;
		return rectangleConstraintDeflector;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		float x = Position.X;
		float num = Position.X + Width;
		float y = Position.Y;
		float num2 = Position.Y + Height;
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			float num3 = ptr->Scale * 0.5f;
			if (ptr->Position.X < x)
			{
				ptr->Position.X = x;
				float num4 = RestitutionCoefficient.Sample();
				ptr->Momentum.X *= 0f - num4;
			}
			else if (ptr->Position.X > num)
			{
				ptr->Position.X = num;
				float num4 = RestitutionCoefficient.Sample();
				ptr->Momentum.X *= 0f - num4;
			}
			if (ptr->Position.Y < y)
			{
				ptr->Position.Y = y;
				float num4 = RestitutionCoefficient.Sample();
				ptr->Momentum.Y *= 0f - num4;
			}
			else if (ptr->Position.Y > num2)
			{
				ptr->Position.Y = num2;
				float num4 = RestitutionCoefficient.Sample();
				ptr->Momentum.Y *= 0f - num4;
			}
		}
	}
}
