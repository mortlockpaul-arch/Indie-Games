using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Quasar.Global;

namespace Quasar.Audios;

public class GroupedAudio : Audio
{
	private List<Audio> audios;

	private int currentAudio;

	private bool autoRandom = true;

	private bool allowRepetition = true;

	public int AudioCount => audios.Count;

	public bool AutoRandom
	{
		get
		{
			return autoRandom;
		}
		set
		{
			autoRandom = value;
		}
	}

	public bool AllowRepetition
	{
		get
		{
			return allowRepetition;
		}
		set
		{
			allowRepetition = value;
		}
	}

	public Audio CurrentAudio => audios[currentAudio];

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
			foreach (Audio audio in audios)
			{
				audio.Volume = value;
			}
		}
	}

	public GroupedAudio(SoundEffect buffer)
	{
		audios = new List<Audio>(4);
		audios.Add(new SimpleAudio(buffer));
		currentAudio = 0;
	}

	public GroupedAudio()
	{
		audios = new List<Audio>(4);
		currentAudio = 0;
	}

	public GroupedAudio(Audio audio)
	{
		audios = new List<Audio>(4);
		audios.Add(audio);
		currentAudio = 0;
	}

	private void SetNextAudioRandom()
	{
		if (!allowRepetition && AudioCount > 1)
		{
			int num = GameMath.Random.Next(AudioCount - 1);
			if (num >= currentAudio)
			{
				num++;
			}
			currentAudio = num;
		}
		else
		{
			currentAudio = GameMath.Random.Next(AudioCount);
		}
	}

	public void AddAudio(SoundEffect buffer)
	{
		audios.Add(new SimpleAudio(buffer));
		if (autoRandom)
		{
			SetNextAudioRandom();
		}
	}

	public void AddAudio(SoundEffect buffer, int instances)
	{
		audios.Add(new PooledAudio(buffer, instances));
		if (autoRandom)
		{
			SetNextAudioRandom();
		}
	}

	public void AddAudio(Audio audio)
	{
		audios.Add(audio);
	}

	public void SetNextAudio(int index)
	{
		currentAudio = index;
	}

	public override void Start()
	{
		if (audios.Count != 0)
		{
			CurrentAudio.Start();
			if (autoRandom)
			{
				SetNextAudioRandom();
			}
		}
	}

	public override void Stop()
	{
		if (audios == null)
		{
			return;
		}
		foreach (Audio audio in audios)
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
		foreach (Audio audio in audios)
		{
			audio.Pause();
		}
	}

	public override void Dispose()
	{
		if (audios != null)
		{
			foreach (Audio audio in audios)
			{
				audio.Dispose();
			}
			audios = null;
		}
		base.Dispose();
	}
}
