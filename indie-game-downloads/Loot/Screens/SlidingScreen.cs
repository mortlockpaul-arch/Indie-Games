using System;
using Eyehook.Framework;
using Microsoft.Xna.Framework;

namespace Loot.Screens;

public class SlidingScreen : Screen
{
	public enum Mode
	{
		Off,
		SlideOn,
		On,
		SlideOff
	}

	protected Vector2 Offset;

	private Mode mode;

	private Vector2 offPos;

	private Vector2 onPos;

	private TimeSpan duration;

	private TimeSpan timer;

	public bool IsOff => mode == Mode.Off;

	public bool IsSlideOn => mode == Mode.SlideOn;

	public bool IsOn => mode == Mode.On;

	public bool IsSlideOff => mode == Mode.SlideOff;

	public SlidingScreen(Vector2 offPos, Vector2 onPos, TimeSpan duration)
		: base(modal: true)
	{
		this.offPos = offPos;
		this.onPos = onPos;
		this.duration = duration;
		timer = TimeSpan.Zero;
		mode = Mode.Off;
		Offset = offPos;
		calcOffset();
	}

	public void SetMode(Mode mode)
	{
		this.mode = mode;
		if (this.mode == Mode.Off)
		{
			timer = TimeSpan.Zero;
		}
		else if (this.mode == Mode.On)
		{
			timer = duration;
		}
		calcOffset();
	}

	public override void update(GameTime gameTime)
	{
		if (mode == Mode.SlideOn)
		{
			timer += gameTime.ElapsedGameTime;
			if (timer >= duration)
			{
				timer = duration;
				mode = Mode.On;
			}
		}
		else if (mode == Mode.SlideOff)
		{
			timer -= gameTime.ElapsedGameTime;
			if (timer <= TimeSpan.Zero)
			{
				timer = TimeSpan.Zero;
				mode = Mode.Off;
			}
		}
		calcOffset();
	}

	private void calcOffset()
	{
		Offset = Vector2.Lerp(offPos, onPos, (float)(timer.TotalSeconds / duration.TotalSeconds));
	}
}
