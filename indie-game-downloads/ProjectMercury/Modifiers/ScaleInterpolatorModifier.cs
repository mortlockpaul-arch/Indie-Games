namespace ProjectMercury.Modifiers;

public class ScaleInterpolatorModifier : Modifier
{
	private float _initialScale;

	private float _middleScale;

	private float _middlePosition;

	private float _finalScale;

	public float InitialScale
	{
		get
		{
			return _initialScale;
		}
		set
		{
			_initialScale = value;
		}
	}

	public float MiddleScale
	{
		get
		{
			return _middleScale;
		}
		set
		{
			_middleScale = value;
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
			_middlePosition = value;
		}
	}

	public float FinalScale
	{
		get
		{
			return _finalScale;
		}
		set
		{
			_finalScale = value;
		}
	}

	public override Modifier DeepCopy()
	{
		ScaleInterpolatorModifier scaleInterpolatorModifier = new ScaleInterpolatorModifier();
		scaleInterpolatorModifier.InitialScale = InitialScale;
		scaleInterpolatorModifier.MiddleScale = MiddleScale;
		scaleInterpolatorModifier.MiddlePosition = MiddlePosition;
		scaleInterpolatorModifier.FinalScale = FinalScale;
		return scaleInterpolatorModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			Particle* ptr2 = ptr - 1;
			if (ptr->Age == ptr2->Age)
			{
				ptr->Scale = ptr2->Scale;
			}
			else if (ptr->Age < MiddlePosition)
			{
				ptr->Scale = InitialScale + (MiddleScale - InitialScale) * (ptr->Age / MiddlePosition);
			}
			else
			{
				ptr->Scale = MiddleScale + (FinalScale - MiddleScale) * ((ptr->Age - MiddlePosition) / (1f - MiddlePosition));
			}
		}
	}
}
