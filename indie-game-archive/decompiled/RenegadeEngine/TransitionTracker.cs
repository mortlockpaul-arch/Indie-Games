using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenegadeEngine;

public class TransitionTracker
{
	protected SpriteBatch spriteBatch;

	public TimeSpan TransitionTime = TimeSpan.Zero;

	public TimeSpan TransitionInTime = new TimeSpan(0, 0, 0, 0, 500);

	public TimeSpan TransitionOutTime = new TimeSpan(0, 0, 0, 0, 500);

	protected TransitionState state = TransitionState.Idle;

	protected float transition;

	public TransitionState State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
			switch (state)
			{
			case TransitionState.In:
				transition = 1f;
				break;
			case TransitionState.Out:
				transition = 0f;
				break;
			case TransitionState.PartialOut:
				transition = 0f;
				break;
			}
		}
	}

	public float Transition => transition;

	public event EventHandler InCompleted;

	public event EventHandler PartialCompleted;

	public event EventHandler OutCompleted;

	public TransitionTracker()
	{
		spriteBatch = EngineManager.GetSpriteBatch;
	}

	public TransitionTracker(TimeSpan inTime, TimeSpan outTime)
	{
		spriteBatch = EngineManager.GetSpriteBatch;
		TransitionInTime = inTime;
		TransitionOutTime = outTime;
	}

	public virtual void Update(GameTime gameTime)
	{
		switch (state)
		{
		case TransitionState.In:
			if (TransitionTime >= TransitionInTime)
			{
				TransitionTime = TimeSpan.Zero;
				state = TransitionState.Idle;
				On_TransInCompleted(new EventArgs());
				break;
			}
			TransitionTime += gameTime.ElapsedGameTime;
			transition = MathHelper.Lerp(1f, 0f, (float)(TransitionTime.TotalSeconds / TransitionInTime.TotalSeconds));
			if (transition < 0f)
			{
				transition = 0f;
			}
			break;
		case TransitionState.Out:
			if (TransitionTime >= TransitionOutTime)
			{
				TransitionTime = TimeSpan.Zero;
				state = TransitionState.Idle;
				On_TransOutCompleted(new EventArgs());
				break;
			}
			TransitionTime += gameTime.ElapsedGameTime;
			transition = MathHelper.Lerp(0f, 1f, (float)(TransitionTime.TotalSeconds / TransitionOutTime.TotalSeconds));
			if (transition > 1f)
			{
				transition = 1f;
			}
			break;
		case TransitionState.PartialOut:
			if (transition >= 0.75f)
			{
				TransitionTime = new TimeSpan(0, 0, 0, (int)(TransitionOutTime.TotalSeconds - TransitionTime.TotalSeconds), (int)(TransitionOutTime.TotalMilliseconds - TransitionTime.TotalMilliseconds));
				state = TransitionState.Idle;
				On_TransPartialCompleted(new EventArgs());
				break;
			}
			TransitionTime += gameTime.ElapsedGameTime;
			transition = MathHelper.Lerp(0f, 1f, (float)(TransitionTime.TotalSeconds / TransitionOutTime.TotalSeconds));
			if (transition > 0.75f)
			{
				transition = 0.75f;
			}
			break;
		}
	}

	public float TransitionIn(GameTime gameTime)
	{
		if (TransitionTime >= TransitionInTime)
		{
			TransitionTime = TimeSpan.Zero;
			state = TransitionState.Idle;
			return 0f;
		}
		TransitionTime += gameTime.ElapsedGameTime;
		return MathHelper.Lerp(1f, 0f, (float)(TransitionTime.TotalSeconds / TransitionInTime.TotalSeconds));
	}

	public float TransitionOut(GameTime gameTime)
	{
		if (TransitionTime >= TransitionOutTime)
		{
			TransitionTime = TimeSpan.Zero;
			state = TransitionState.Idle;
			return 1f;
		}
		TransitionTime += gameTime.ElapsedGameTime;
		return MathHelper.Lerp(0f, 1f, (float)(TransitionTime.TotalSeconds / TransitionOutTime.TotalSeconds));
	}

	protected internal virtual void On_TransInCompleted(EventArgs e)
	{
		if (InCompleted != null)
		{
			InCompleted(this, e);
		}
	}

	protected internal virtual void On_TransPartialCompleted(EventArgs e)
	{
		if (PartialCompleted != null)
		{
			PartialCompleted(this, e);
		}
	}

	protected internal virtual void On_TransOutCompleted(EventArgs e)
	{
		if (OutCompleted != null)
		{
			OutCompleted(this, e);
		}
	}
}
