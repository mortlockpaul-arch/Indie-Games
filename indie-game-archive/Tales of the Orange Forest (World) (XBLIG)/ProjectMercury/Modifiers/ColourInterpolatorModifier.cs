#define DEBUG
using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public class ColourInterpolatorModifier : Modifier
{
	private float _middlePosition;

	public Vector3 InitialColour { get; set; }

	public Vector3 MiddleColour { get; set; }

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

	public Vector3 FinalColour { get; set; }

	public override Modifier DeepCopy()
	{
		ColourInterpolatorModifier colourInterpolatorModifier = new ColourInterpolatorModifier();
		colourInterpolatorModifier.FinalColour = FinalColour;
		colourInterpolatorModifier.InitialColour = InitialColour;
		colourInterpolatorModifier.MiddleColour = MiddleColour;
		colourInterpolatorModifier.MiddlePosition = MiddlePosition;
		return colourInterpolatorModifier;
	}

	protected internal unsafe override void Process(float elapsedSeconds, Particle* particle, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particle - 1;
			if (particle->Age == ptr->Age)
			{
				particle->Colour.X = ptr->Colour.X;
				particle->Colour.Y = ptr->Colour.Y;
				particle->Colour.Z = ptr->Colour.Z;
			}
			else
			{
				Vector3 vector;
				Vector3 vector2;
				float num;
				if (particle->Age < MiddlePosition)
				{
					vector = InitialColour;
					vector2 = MiddleColour;
					num = particle->Age / MiddlePosition;
				}
				else
				{
					vector = MiddleColour;
					vector2 = FinalColour;
					num = (particle->Age - MiddlePosition) / (1f - MiddlePosition);
				}
				particle->Colour.X = vector.X + (vector2.X - vector.X) * num;
				particle->Colour.Y = vector.Y + (vector2.Y - vector.Y) * num;
				particle->Colour.Z = vector.Z + (vector2.Z - vector.Z) * num;
			}
			particle++;
		}
	}
}
