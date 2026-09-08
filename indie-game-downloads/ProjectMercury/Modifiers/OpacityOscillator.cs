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
