#define DEBUG
namespace ProjectMercury.Modifiers;

public class OpacityOscillator : Modifier
{
	private float TotalSeconds;

	private float _frequency;

	private float _minimum;

	private float _maximum;

	public float Frequency
	{
		get
		{
			return _frequency;
		}
		set
		{
			Guard.ArgumentNotFinite("Frequency", value);
			Guard.ArgumentLessThan("Frequency", value, float.Epsilon);
			_frequency = value;
		}
	}

	public float MinimumOpacity
	{
		get
		{
			return _minimum;
		}
		set
		{
			Guard.ArgumentNotFinite("MinimumOpacity", value);
			Guard.ArgumentLessThan("MinimumOpacity", value, 0f);
			Guard.ArgumentGreaterThan("MinimumOpacity", value, 1f);
			_minimum = value;
		}
	}

	public float MaximumOpacity
	{
		get
		{
			return _maximum;
		}
		set
		{
			Guard.ArgumentNotFinite("MaximumOpacity", value);
			Guard.ArgumentLessThan("MaximumOpacity", value, 0f);
			Guard.ArgumentGreaterThan("MaximumOpacity", value, 1f);
			_maximum = value;
		}
	}

	public override Modifier DeepCopy()
	{
		OpacityOscillator opacityOscillator = new OpacityOscillator();
		opacityOscillator.Frequency = Frequency;
		opacityOscillator.MinimumOpacity = MinimumOpacity;
		opacityOscillator.MaximumOpacity = MaximumOpacity;
		return opacityOscillator;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		TotalSeconds += dt;
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			float num = TotalSeconds - ptr->Inception;
			float num2 = Calculator.Sin(num * (Frequency * 3f));
			ptr->Colour.W = (MaximumOpacity - MinimumOpacity) * num2 + MinimumOpacity;
		}
	}
}
