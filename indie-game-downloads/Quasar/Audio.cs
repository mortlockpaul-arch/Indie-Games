using System;
using Quasar.Global;

namespace Quasar;

public abstract class Audio : IDisposable
{
	private static float soundVolume = 1f;

	private static float musicVolume = 1f;

	public static float SoundVolume
	{
		get
		{
			return soundVolume;
		}
		set
		{
			soundVolume = value;
		}
	}

	public static float MusicVolume
	{
		get
		{
			return musicVolume;
		}
		set
		{
			musicVolume = value;
		}
	}

	public abstract bool Loop { get; set; }

	public abstract float Speed { get; set; }

	public abstract float Volume { get; set; }

	public abstract float Pan { get; set; }

	public abstract bool Playing { get; }

	public abstract void Start();

	public void Start(float pitchVariation)
	{
		pitchVariation = GameMath.Saturate(pitchVariation);
		Speed = 1f + GameMath.Random.NextFloat(0f - pitchVariation, pitchVariation);
		Start();
	}

	public abstract void Stop();

	public abstract void Pause();

	public virtual void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	~Audio()
	{
		Dispose();
	}
}
