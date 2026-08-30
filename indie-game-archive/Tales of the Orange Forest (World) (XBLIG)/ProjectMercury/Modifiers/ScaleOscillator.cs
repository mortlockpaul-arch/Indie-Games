#define DEBUG
namespace ProjectMercury.Modifiers;

public class ScaleOscillator : Modifier
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

	public float MinimumScale
	{
		get
		{
			return _minimum;
		}
		set
		{
			Guard.ArgumentNotFinite("MinimumScale", value);
			Guard.ArgumentLessThan("MinimumScale", value, 0f);
			_minimum = value;
		}
	}

	public float MaximumScale
	{
		get
		{
			return _maximum;
		}
		set
		{
			Guard.ArgumentNotFinite("MaximumScale", value);
			Guard.ArgumentLessThan("MaximumScale", value, 0f);
			_maximum = value;
		}
	}

	public override Modifier DeepCopy()
	{
		ScaleOscillator scaleOscillator = new ScaleOscillator();
		scaleOscillator.Frequency = Frequency;
		scaleOscillator.MinimumScale = MinimumScale;
		scaleOscillator.MaximumScale = MaximumScale;
		return scaleOscillator;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		TotalSeconds += dt;
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			float num = TotalSeconds - ptr->Inception;
			float num2 = Calculator.Sin(num * (Frequency * 3f));
			ptr->Scale = (MaximumScale - MinimumScale) * num2 + MinimumScale;
		}
	}
}
