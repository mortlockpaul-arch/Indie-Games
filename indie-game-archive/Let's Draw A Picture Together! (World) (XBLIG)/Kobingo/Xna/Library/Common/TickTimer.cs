using System;
using Microsoft.Xna.Framework;

namespace Kobingo.Xna.Library.Common;

public class TickTimer
{
	public TimeSpan Interval { get; set; }

	public TimeSpan Elapsed { get; private set; }

	public TimeSpan Left => Interval - Elapsed;

	public bool Enabled { get; set; }

	public event EventHandler Tick;

	public TickTimer(TimeSpan interval)
	{
		Enabled = true;
		Interval = interval;
	}

	public void Update(GameTime gameTime)
	{
		if (!Enabled)
		{
			return;
		}
		Elapsed += gameTime.ElapsedGameTime;
		while (Elapsed >= Interval)
		{
			if (Tick != null)
			{
				Tick(this, EventArgs.Empty);
			}
			Elapsed -= Interval;
		}
	}

	public void Reset()
	{
		Elapsed = TimeSpan.Zero;
	}
}
