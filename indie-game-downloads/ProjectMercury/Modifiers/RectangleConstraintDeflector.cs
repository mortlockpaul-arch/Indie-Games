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
			_ = ptr->Scale;
			if (ptr->Position.X < x)
			{
				ptr->Position.X = x;
				float num3 = RestitutionCoefficient.Sample();
				ptr->Momentum.X *= 0f - num3;
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
				float num5 = RestitutionCoefficient.Sample();
				ptr->Momentum.Y *= 0f - num5;
			}
			else if (ptr->Position.Y > num2)
			{
				ptr->Position.Y = num2;
				float num6 = RestitutionCoefficient.Sample();
				ptr->Momentum.Y *= 0f - num6;
			}
		}
	}
}
