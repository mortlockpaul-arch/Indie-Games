#define DEBUG
namespace ProjectMercury.Modifiers;

public class OpacityInterpolatorModifier : Modifier
{
	private float _initialOpacity;

	private float _middleOpacity;

	private float _middlePosition;

	private float _finalOpacity;

	public float InitialOpacity
	{
		get
		{
			return _initialOpacity;
		}
		set
		{
			Guard.ArgumentOutOfRange("InitialOpacity", value, 0f, 1f);
			_initialOpacity = value;
		}
	}

	public float MiddleOpacity
	{
		get
		{
			return _middleOpacity;
		}
		set
		{
			Guard.ArgumentOutOfRange("MiddleOpacity", value, 0f, 1f);
			_middleOpacity = value;
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

	public float FinalOpacity
	{
		get
		{
			return _finalOpacity;
		}
		set
		{
			Guard.ArgumentOutOfRange("FinalOpacity", value, 0f, 1f);
			_finalOpacity = value;
		}
	}

	public override Modifier DeepCopy()
	{
		OpacityInterpolatorModifier opacityInterpolatorModifier = new OpacityInterpolatorModifier();
		opacityInterpolatorModifier.InitialOpacity = InitialOpacity;
		opacityInterpolatorModifier.MiddleOpacity = MiddleOpacity;
		opacityInterpolatorModifier.MiddlePosition = MiddlePosition;
		opacityInterpolatorModifier.FinalOpacity = FinalOpacity;
		return opacityInterpolatorModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			Particle* ptr2 = ptr - 1;
			if (ptr->Age == ptr2->Age)
			{
				ptr->Colour.W = ptr2->Colour.W;
			}
			else if (ptr->Age < MiddlePosition)
			{
				ptr->Colour.W = InitialOpacity + (MiddleOpacity - InitialOpacity) * (ptr->Age / MiddlePosition);
			}
			else
			{
				ptr->Colour.W = MiddleOpacity + (FinalOpacity - MiddleOpacity) * ((ptr->Age - MiddlePosition) / (1f - MiddlePosition));
			}
		}
	}
}
