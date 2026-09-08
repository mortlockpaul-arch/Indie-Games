using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public sealed class ColourMergeModifier : Modifier
{
	public Vector3 MergeColour;

	public override Modifier DeepCopy()
	{
		ColourMergeModifier colourMergeModifier = new ColourMergeModifier();
		colourMergeModifier.MergeColour = MergeColour;
		return colourMergeModifier;
	}

	protected internal unsafe override void Process(float elapsedSeconds, Particle* particle, int count)
	{
		for (int i = 0; i < count; i++)
		{
			float num = particle->Age * 0.07f;
			float num2 = 1f - num;
			particle->Colour.X = particle->Colour.X * num2 + MergeColour.X * num;
			particle->Colour.Y = particle->Colour.Y * num2 + MergeColour.Y * num;
			particle->Colour.Z = particle->Colour.Z * num2 + MergeColour.Z * num;
			particle++;
		}
	}
}
