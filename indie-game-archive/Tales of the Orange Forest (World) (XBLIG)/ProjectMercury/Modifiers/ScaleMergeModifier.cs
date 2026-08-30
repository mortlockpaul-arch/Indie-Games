#define DEBUG
namespace ProjectMercury.Modifiers;

public sealed class ScaleMergeModifier : Modifier
{
	private float _mergeScale;

	public float MergeScale
	{
		get
		{
			return _mergeScale;
		}
		set
		{
			Guard.ArgumentNotFinite("MergeScale", value);
			Guard.ArgumentLessThan("MergeScale", value, 0f);
			_mergeScale = value;
		}
	}

	public override Modifier DeepCopy()
	{
		ScaleMergeModifier scaleMergeModifier = new ScaleMergeModifier();
		scaleMergeModifier.MergeScale = MergeScale;
		return scaleMergeModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			float num = ptr->Age * 0.07f;
			float num2 = 1f - num;
			ptr->Scale = ptr->Scale * num2 + MergeScale * num;
		}
	}
}
