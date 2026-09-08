using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public sealed class BoundingRectModifier : Modifier
{
	public BoundingRect BoundingRect { get; private set; }

	public float Padding { get; set; }

	public override Modifier DeepCopy()
	{
		BoundingRectModifier boundingRectModifier = new BoundingRectModifier();
		boundingRectModifier.Padding = Padding;
		return boundingRectModifier;
	}

	protected internal unsafe override void Process(float elapsedSeconds, Particle* particle, int count)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < count; i++)
		{
			num = ((particle->Position.X < num) ? particle->Position.X : num);
			num3 = ((particle->Position.X > num3) ? particle->Position.X : num3);
			num2 = ((particle->Position.Y < num2) ? particle->Position.Y : num2);
			num4 = ((particle->Position.Y > num4) ? particle->Position.Y : num4);
			particle++;
		}
		BoundingRect = new BoundingRect
		{
			Min = new Vector2
			{
				X = num - Padding,
				Y = num2 - Padding
			},
			Max = new Vector2
			{
				X = num3 + Padding,
				Y = num4 + Padding
			}
		};
	}
}
