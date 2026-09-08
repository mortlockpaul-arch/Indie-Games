using System;
using Microsoft.Xna.Framework.Audio;
using Quasar.Global;

namespace Quasar.Audios;

public class SimpleAudio : Audio
{
	private SoundEffect buffer;

	private SoundEffectInstance instance;

	private bool instanceError;

	private bool loop;

	private float speed = 1f;

	private float pan;

	private static int soundPlays;

	private float volume = 1f;

	public override bool Loop
	{
		get
		{
			return loop;
		}
		set
		{
			loop = value;
		}
	}

	public override float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			speed = value;
			if (instance != null)
			{
				instance.Pitch = value - 1f;
			}
		}
	}

	public override float Pan
	{
		get
		{
			return pan;
		}
		set
		{
			pan = value;
			if (instance != null)
			{
				instance.Pan = value;
			}
		}
	}

	public override bool Playing
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			return instance.State == SoundState.Playing;
		}
	}

	public override float Volume
	{
		get
		{
			return volume;
		}
		set
		{
			volume = value;
			if (instance != null)
			{
				instance.Volume = volume * Audio.SoundVolume;
			}
		}
	}

	public SimpleAudio(SoundEffect buffer)
	{
		this.buffer = buffer;
		instance = null;
	}

	public override void Start()
	{
		if (Timer.DefaultTimer.TotalTime < 500L)
		{
			soundPlays++;
			return;
		}
		try
		{
			if (buffer != null && !buffer.IsDisposed)
			{
				if (instance != null)
				{
					if (!instance.IsDisposed)
					{
						goto IL_0085;
					}
					instance = null;
				}
				instance = buffer.CreateInstance();
				instance.IsLooped = loop;
				goto IL_0085;
			}
			instanceError = true;
			goto end_IL_0025;
			IL_0085:
			instance.Volume = volume * Audio.SoundVolume;
			instance.Pitch = speed - 1f;
			instance.Pan = pan;
			instance.Play();
			instanceError = false;
			end_IL_0025:;
		}
		catch (Exception)
		{
			instance = null;
			instanceError = true;
		}
		soundPlays++;
	}

	public override void Stop()
	{
		if (instance != null && !instance.IsDisposed)
		{
			instance.Stop();
		}
	}

	public override void Pause()
	{
		if (instance != null && !instance.IsDisposed)
		{
			instance.Pause();
		}
	}

	~SimpleAudio()
	{
		Dispose();
	}

	public override void Dispose()
	{
		if (instance != null)
		{
			instance.Dispose();
		}
		instance = null;
		buffer = null;
		base.Dispose();
	}
}
