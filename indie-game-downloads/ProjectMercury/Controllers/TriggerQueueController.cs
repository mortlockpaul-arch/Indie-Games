using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectMercury.Controllers;

public sealed class TriggerQueueController : Controller
{
	public Queue<Vector2> QueuedTriggers { get; private set; }

	public TriggerQueueController()
	{
		QueuedTriggers = new Queue<Vector2>();
	}

	protected internal override void Trigger(ref Vector2 position)
	{
		QueuedTriggers.Enqueue(position);
	}

	protected internal override void Update(float deltaSeconds)
	{
		if (base.ParticleEffect != null && QueuedTriggers.Count > 0 && base.ParticleEffect.ActiveParticlesCount == 0)
		{
			Vector2 triggerPosition = QueuedTriggers.Dequeue();
			for (int i = 0; i < base.ParticleEffect.Count; i++)
			{
				base.ParticleEffect[i].Trigger(ref triggerPosition);
			}
		}
		base.Update(deltaSeconds);
	}
}
