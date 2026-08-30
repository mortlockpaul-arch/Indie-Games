using Microsoft.Xna.Framework;

namespace ProjectMercury.Controllers;

public abstract class Controller
{
	public ParticleEffect ParticleEffect { get; internal set; }

	protected internal virtual void Trigger(ref Vector2 position)
	{
		if (ParticleEffect != null)
		{
			for (int i = 0; i < ParticleEffect.Count; i++)
			{
				ParticleEffect[i].Trigger(ref position);
			}
		}
	}

	protected internal virtual void Update(float deltaSeconds)
	{
		if (ParticleEffect != null)
		{
			for (int i = 0; i < ParticleEffect.Count; i++)
			{
				ParticleEffect[i].Update(deltaSeconds);
			}
		}
	}
}
