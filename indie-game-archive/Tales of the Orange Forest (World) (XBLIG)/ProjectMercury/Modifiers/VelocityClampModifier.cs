#define DEBUG
namespace ProjectMercury.Modifiers;

public class VelocityClampModifier : Modifier
{
	private float _maximumVelocity;

	private float SquareMaximumVelocity;

	public float MaximumVelocity
	{
		get
		{
			return _maximumVelocity;
		}
		set
		{
			Guard.ArgumentNotFinite("MaximumVelocity", value);
			Guard.ArgumentLessThan("MaximumVelocity", value, 0f);
			_maximumVelocity = value;
			SquareMaximumVelocity = value * value;
		}
	}

	public override Modifier DeepCopy()
	{
		VelocityClampModifier velocityClampModifier = new VelocityClampModifier();
		velocityClampModifier.MaximumVelocity = MaximumVelocity;
		return velocityClampModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			float num = ptr->Velocity.X * ptr->Velocity.X + ptr->Velocity.Y * ptr->Velocity.Y;
			if (num > SquareMaximumVelocity)
			{
				float num2 = Calculator.Sqrt(num);
				ptr->Velocity.X = ptr->Velocity.X / num2 * MaximumVelocity;
				ptr->Velocity.Y = ptr->Velocity.Y / num2 * MaximumVelocity;
			}
		}
	}
}
