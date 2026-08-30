#define DEBUG
namespace ProjectMercury.Modifiers;

public sealed class OpacityModifier : Modifier
{
	private float _initial;

	private float _ultimate;

	public float Initial
	{
		get
		{
			return _initial;
		}
		set
		{
			Guard.ArgumentNotFinite("Initial", value);
			Guard.ArgumentLessThan("Initial", value, 0f);
			Guard.ArgumentGreaterThan("Initial", value, 1f);
			_initial = value;
		}
	}

	public float Ultimate
	{
		get
		{
			return _ultimate;
		}
		set
		{
			Guard.ArgumentNotFinite("Ultimate", value);
			Guard.ArgumentLessThan("Ultimate", value, 0f);
			Guard.ArgumentGreaterThan("Ultimate", value, 1f);
			_ultimate = value;
		}
	}

	public override Modifier DeepCopy()
	{
		OpacityModifier opacityModifier = new OpacityModifier();
		opacityModifier.Initial = Initial;
		opacityModifier.Ultimate = Ultimate;
		return opacityModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			ptr->Colour.W = Initial + (Ultimate - Initial) * ptr->Age;
		}
	}
}
