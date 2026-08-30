using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Maximinus;

public class EngineSound
{
	public enum State
	{
		On,
		TransitionOn,
		TransitionOff,
		Off
	}

	private const float transitionDurationSecondsDefault = 0.5f;

	private SoundEffectInstance sndInstance;

	private float transitionDurationSeconds;

	private double transitionStartTime;

	private float volume;

	private float maxVolume;

	private State state;

	public EngineSound(SoundEffectInstance sndPlane, float maxVolume, float selectionDurationSeconds)
	{
		sndInstance = sndPlane;
		sndInstance.IsLooped = true;
		sndInstance.Play();
		sndInstance.Pause();
		this.maxVolume = maxVolume;
		transitionDurationSeconds = selectionDurationSeconds;
		state = State.Off;
		volume = 0f;
		sndInstance.Volume = volume * maxVolume;
	}

	public EngineSound(SoundEffectInstance sndPlane, float maxVolume)
		: this(sndPlane, maxVolume, 0.5f)
	{
	}

	public void UpdateOld(GameTime gameTime, bool isAlive, float turnRatio)
	{
		sndInstance.Pitch = 0f + turnRatio * 0.6f;
		Update(gameTime, isAlive, 1f);
	}

	public void UpdateNew(GameTime gameTime, bool isAlive, float rpmRatio, float volumeParameter)
	{
		sndInstance.Pitch = rpmRatio;
		Update(gameTime, isAlive, volumeParameter);
	}

	public void Update(GameTime gameTime, bool isAlive, float volumeParameter)
	{
		if (!isAlive && (state == State.On || state == State.TransitionOn))
		{
			Stop(gameTime);
			return;
		}
		if (isAlive && (state == State.Off || state == State.TransitionOff))
		{
			Start(gameTime);
			return;
		}
		if (state == State.TransitionOn || state == State.TransitionOff)
		{
			float num = (float)(gameTime.TotalGameTime.TotalSeconds - transitionStartTime);
			if (num >= transitionDurationSeconds)
			{
				if (state == State.TransitionOff)
				{
					volume = 0f;
					state = State.Off;
					sndInstance.Pause();
				}
				else
				{
					volume = 1f;
					state = State.On;
				}
			}
			else
			{
				float amount = ((state != State.TransitionOn) ? (1f - num / transitionDurationSeconds) : (num / transitionDurationSeconds));
				volume = MathHelper.SmoothStep(0f, 1f, amount);
			}
		}
		sndInstance.Volume = MathHelper.Lerp(sndInstance.Volume, volume * volumeParameter * maxVolume, 0.1f);
	}

	private void Start(GameTime gameTime)
	{
		transitionStartTime = gameTime.TotalGameTime.TotalSeconds;
		state = State.TransitionOn;
		sndInstance.Resume();
	}

	private void Stop(GameTime gameTime)
	{
		transitionStartTime = gameTime.TotalGameTime.TotalSeconds;
		state = State.TransitionOff;
	}
}
