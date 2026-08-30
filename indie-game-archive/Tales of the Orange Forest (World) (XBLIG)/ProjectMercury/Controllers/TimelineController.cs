using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectMercury.Controllers;

public sealed class TimelineController : Controller
{
	public float TotalSeconds { get; private set; }

	public List<TimelineEvent> Timeline { get; private set; }

	private List<TimelineEvent> EventQueue { get; set; }

	public TimelineController()
	{
		Timeline = new List<TimelineEvent>();
		EventQueue = new List<TimelineEvent>();
	}

	protected internal override void Trigger(ref Vector2 position)
	{
		foreach (TimelineEvent item in Timeline)
		{
			EventQueue.Add(new TimelineEvent
			{
				EmitterName = item.EmitterName,
				TimeOffset = item.TimeOffset + TotalSeconds,
				TriggerPosition = position
			});
		}
	}

	protected internal override void Update(float deltaSeconds)
	{
		TotalSeconds += deltaSeconds;
		if (EventQueue.Count > 0)
		{
			for (int num = EventQueue.Count - 1; num >= 0; num--)
			{
				TimelineEvent timelineEvent = EventQueue[num];
				if (timelineEvent.TimeOffset <= TotalSeconds)
				{
					base.ParticleEffect[timelineEvent.EmitterName].Trigger(ref timelineEvent.TriggerPosition);
					EventQueue.RemoveAt(num);
				}
			}
		}
		base.Update(deltaSeconds);
	}
}
