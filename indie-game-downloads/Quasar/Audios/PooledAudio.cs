using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Quasar.Audios;

public class PooledAudio : Audio
{
	private const int DEFAULT_INSTANCES = 3;

	private List<SimpleAudio> audios;

	private int currentAudio;

	public int Count => audios.Count;

	public int ActiveSounds
	{
		get
		{
			int num = 0;
			for (int i = 0; i < audios.Count; i++)
			{
				if (audios[i].Playing)
				{
					num++;
				}
			}
			return num;
		}
	}

	public float Usage => (float)ActiveSounds / (float)audios.Count;

	private SimpleAudio CurrentAudio => audios[currentAudio];

	public override bool Loop
	{
		get
		{
			return CurrentAudio.Loop;
		}
		set
		{
			CurrentAudio.Loop = value;
		}
	}

	public override float Pan
	{
		get
		{
			return CurrentAudio.Pan;
		}
		set
		{
			CurrentAudio.Pan = value;
		}
	}

	public override bool Playing => CurrentAudio.Playing;

	public override float Speed
	{
		get
		{
			return CurrentAudio.Speed;
		}
		set
		{
			CurrentAudio.Speed = value;
		}
	}

	public override float Volume
	{
		get
		{
			return CurrentAudio.Volume;
		}
		set
		{
			foreach (SimpleAudio audio in audios)
			{
				audio.Volume = value;
			}
		}
	}

	public PooledAudio(SoundEffect buffer)
		: this(buffer, 3)
	{
	}

	public PooledAudio(SoundEffect buffer, int instances)
	{
		audios = new List<SimpleAudio>(instances);
		for (int i = 0; i < instances; i++)
		{
			audios.Add(new SimpleAudio(buffer));
		}
		currentAudio = 0;
	}

	private void switchToNextAudio()
	{
		currentAudio = (currentAudio + 1) % audios.Count;
	}

	public override void Start()
	{
		try
		{
			if (audios != null && audios.Count > 0)
			{
				if (currentAudio < 0 || currentAudio >= audios.Count)
				{
					currentAudio = 0;
				}
				CurrentAudio?.Start();
				switchToNextAudio();
			}
		}
		catch (Exception)
		{
			currentAudio = 0;
		}
	}

	public override void Stop()
	{
		if (audios == null)
		{
			return;
		}
		foreach (SimpleAudio audio in audios)
		{
			audio.Stop();
		}
	}

	public override void Pause()
	{
		if (audios == null)
		{
			return;
		}
		foreach (SimpleAudio audio in audios)
		{
			audio.Pause();
		}
	}

	public override void Dispose()
	{
		if (audios != null)
		{
			foreach (SimpleAudio audio in audios)
			{
				audio.Dispose();
			}
			audios = null;
		}
		base.Dispose();
	}
}
