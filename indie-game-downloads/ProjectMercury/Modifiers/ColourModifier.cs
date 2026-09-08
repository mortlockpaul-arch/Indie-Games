using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public sealed class ColourModifier : Modifier
{
	public Vector3 InitialColour;

	public Vector3 UltimateColour;

	public override Modifier DeepCopy()
	{
		ColourModifier colourModifier = new ColourModifier();
		colourModifier.InitialColour = InitialColour;
		colourModifier.UltimateColour = UltimateColour;
		return colourModifier;
	}

	protected internal unsafe override void Process(float elapsedSeconds, Particle* particle, int count)
	{
		particle->Colour.X = InitialColour.X + (UltimateColour.X - InitialColour.X) * particle->Age;
		particle->Colour.Y = InitialColour.Y + (UltimateColour.Y - InitialColour.Y) * particle->Age;
		particle->Colour.Z = InitialColour.Z + (UltimateColour.Z - InitialColour.Z) * particle->Age;
		Particle* ptr = particle;
		particle++;
		for (int i = 1; i < count; i++)
		{
			if (particle->Age < ptr->Age)
			{
				particle->Colour.X = InitialColour.X + (UltimateColour.X - InitialColour.X) * particle->Age;
				particle->Colour.Y = InitialColour.Y + (UltimateColour.Y - InitialColour.Y) * particle->Age;
				particle->Colour.Z = InitialColour.Z + (UltimateColour.Z - InitialColour.Z) * particle->Age;
			}
			else
			{
				particle->Colour.X = ptr->Colour.X;
				particle->Colour.Y = ptr->Colour.Y;
				particle->Colour.Z = ptr->Colour.Z;
			}
			ptr++;
			particle++;
		}
	}
}
