using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Game.Audio;

public class MusicManager
{
	public GameAudio audio;

	public AudioCategory category;

	public MusicTrack[] tracks;

	public MusicTrack current;

	private float _volume;

	public float volume
	{
		get
		{
			return _volume;
		}
		set
		{
			_volume = value;
			category.SetVolume(_volume);
		}
	}

	public MusicManager(GameAudio oAudio)
	{
		audio = oAudio;
		Init();
	}

	private void Init()
	{
		tracks = new MusicTrack[5];
		tracks[0] = new MusicTrack(this, "Main Title", new AudioEventCue[1]
		{
			new AudioEventCue(audio, "Music_0")
		});
		tracks[1] = new MusicTrack(this, "Mystery Garden", new AudioEventCue[1]
		{
			new AudioEventCue(audio, "Music_1")
		});
		tracks[2] = new MusicTrack(this, "Winter Wonderland", new AudioEventCue[1]
		{
			new AudioEventCue(audio, "Music_2")
		});
		tracks[3] = new MusicTrack(this, "Urban Rat Race", new AudioEventCue[1]
		{
			new AudioEventCue(audio, "Music_3")
		});
		tracks[4] = new MusicTrack(this, "Outer Space", new AudioEventCue[1]
		{
			new AudioEventCue(audio, "Music_4")
		});
		category = audio.audioEngine.GetCategory("Music");
		volume = 1f;
	}

	public void Update(GameTime oGameTime)
	{
	}

	public void Dispose()
	{
	}

	public void Set(int xIndex)
	{
		if (current != null)
		{
			current.Stop();
		}
		if (xIndex >= 0)
		{
			current = tracks[xIndex];
			current.Play();
		}
	}

	public void Stop()
	{
		if (current != null)
		{
			current.Stop();
		}
	}

	public void Play()
	{
		if (current != null)
		{
			current.Play();
		}
	}

	public void Pause()
	{
		if (current != null)
		{
			current.Pause();
		}
	}

	public void Resume()
	{
		if (current != null)
		{
			current.Resume();
		}
	}

	public int IndexOf(MusicTrack oTrack)
	{
		int result = -1;
		for (int i = 0; i < tracks.Length; i++)
		{
			if (tracks[i] == oTrack)
			{
				result = i;
				break;
			}
		}
		return result;
	}
}
