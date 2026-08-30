#define DEBUG
using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public class ForceInterpolatorModifier : Modifier
{
	private Vector2 _initialForce;

	private Vector2 _middleForce;

	private float _middlePosition;

	private Vector2 _finalForce;

	public Vector2 InitialForce
	{
		get
		{
			return _initialForce;
		}
		set
		{
			_initialForce = value;
		}
	}

	public Vector2 MiddleForce
	{
		get
		{
			return _middleForce;
		}
		set
		{
			_middleForce = value;
		}
	}

	public float MiddlePosition
	{
		get
		{
			return _middlePosition;
		}
		set
		{
			Guard.ArgumentOutOfRange("MiddlePosition", value, 0f, 1f);
			_middlePosition = value;
		}
	}

	public Vector2 FinalForce
	{
		get
		{
			return _finalForce;
		}
		set
		{
			_finalForce = value;
		}
	}

	public override Modifier DeepCopy()
	{
		ForceInterpolatorModifier forceInterpolatorModifier = new ForceInterpolatorModifier();
		forceInterpolatorModifier.InitialForce = InitialForce;
		forceInterpolatorModifier.MiddleForce = MiddleForce;
		forceInterpolatorModifier.MiddlePosition = MiddlePosition;
		forceInterpolatorModifier.FinalForce = FinalForce;
		return forceInterpolatorModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			Particle* ptr2 = ptr - 1;
			if (ptr->Age == ptr2->Age)
			{
				ptr->Velocity.X = ptr2->Velocity.X;
				ptr->Velocity.Y = ptr2->Velocity.Y;
			}
			else if (ptr->Age < MiddlePosition)
			{
				float num = ptr->Age / MiddlePosition;
				ptr->Velocity.X += InitialForce.X + (MiddleForce.X - InitialForce.X) * num;
				ptr->Velocity.Y += InitialForce.Y + (MiddleForce.Y - InitialForce.Y) * num;
			}
			else
			{
				float num = (ptr->Age - MiddlePosition) / (1f - MiddlePosition);
				ptr->Velocity.X += MiddleForce.X + (FinalForce.X - MiddleForce.X) * num;
				ptr->Velocity.Y += MiddleForce.Y + (FinalForce.Y - MiddleForce.Y) * num;
			}
		}
	}
}
