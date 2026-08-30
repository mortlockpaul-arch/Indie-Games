using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public sealed class PlatformModifier : Modifier
{
	public List<BoundingBox> Platforms { get; set; }

	public PlatformModifier()
	{
		Platforms = new List<BoundingBox>();
	}

	public override Modifier DeepCopy()
	{
		PlatformModifier platformModifier = new PlatformModifier();
		platformModifier.Platforms.AddRange(Platforms);
		return platformModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			Vector3 point = new Vector3(ptr->Position, 0f);
			for (int j = 0; j < Platforms.Count; j++)
			{
				Platforms[j].Contains(ref point, out var result);
				if (result == ContainmentType.Contains)
				{
					ptr->Momentum = Vector2.Zero;
					return;
				}
			}
		}
	}
}
