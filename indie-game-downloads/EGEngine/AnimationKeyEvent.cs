using System;

namespace EGEngine;

public class AnimationKeyEvent
{
	private bool toggled;

	private int frame;

	private TimeSpan time;

	private EventHandler<AnimationEventArgs> callback;

	public void Set(int f, TimeSpan t, EventHandler<AnimationEventArgs> e)
	{
		toggled = false;
		frame = f;
		time = t;
		callback = e;
	}

	public void Update(TimeSpan t)
	{
		if (time > t)
		{
			toggled = false;
		}
		else if (!toggled)
		{
			toggled = true;
			callback(this, null);
		}
	}
}
