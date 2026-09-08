using Microsoft.Xna.Framework.Audio;

namespace Deep_waters;

public class AudioManager
{
	private AudioEngine engine;

	private SoundBank soundBank;

	private WaveBank waveBank;

	private AudioCategory audiocategory;

	public Cue BGMCue;

	public Cue BGfxCue;

	public Cue[] sounds;

	public float BGMVolume = 10f;

	public float FXvolume = 10f;

	public AudioManager(string xgs, string sb, string wb)
	{
		engine = new AudioEngine("Content\\Audio\\" + xgs + ".xgs");
		soundBank = new SoundBank(engine, "Content\\Audio\\" + sb + ".xsb");
		waveBank = new WaveBank(engine, "Content\\Audio\\" + wb + ".xwb");
		sounds = new Cue[32];
		audiocategory = engine.GetCategory("Music");
		audiocategory.SetVolume(BGMVolume);
		audiocategory = engine.GetCategory("Fx");
		audiocategory.SetVolume(FXvolume);
	}

	public bool isplayngsfx(string soundname)
	{
		for (int i = 0; i < sounds.Length; i++)
		{
			if (sounds[i] != null && sounds[i].Name == soundname && sounds[i].IsPlaying)
			{
				return true;
			}
		}
		return false;
	}

	public void playsound(string soundname)
	{
		for (int i = 0; i < sounds.Length; i++)
		{
			if (sounds[i] == null)
			{
				sounds[i] = soundBank.GetCue(soundname);
				sounds[i].Play();
				break;
			}
		}
	}

	public void pausesound(string soundname)
	{
		for (int i = 0; i < sounds.Length; i++)
		{
			if (sounds[i] != null && sounds[i].Name == soundname)
			{
				if (sounds[i].IsPlaying)
				{
					sounds[i].Pause();
				}
				break;
			}
		}
	}

	public void resumesound(string soundname)
	{
		for (int i = 0; i < sounds.Length; i++)
		{
			if (sounds[i] != null && sounds[i].Name == soundname)
			{
				if (sounds[i].IsPaused)
				{
					sounds[i].Resume();
				}
				break;
			}
		}
	}

	public void PlayMusic(string musicname)
	{
		if (BGMCue != null && BGMCue.IsPlaying)
		{
			BGMCue.Stop(AudioStopOptions.AsAuthored);
		}
		BGMCue = soundBank.GetCue(musicname);
		BGMCue.Play();
	}

	public void stopMusic()
	{
		if (BGMCue != null)
		{
			BGMCue.Stop(AudioStopOptions.AsAuthored);
		}
	}

	public void RefreshVolume()
	{
		audiocategory = engine.GetCategory("Music");
		if (BGMVolume > 0f)
		{
			audiocategory.SetVolume(BGMVolume);
		}
		audiocategory = engine.GetCategory("Fx");
		if (FXvolume > 0f)
		{
			audiocategory.SetVolume(FXvolume);
		}
	}

	public void update()
	{
		RefreshVolume();
		for (int i = 0; i < sounds.Length; i++)
		{
			if (sounds[i] != null && sounds[i].IsStopped)
			{
				sounds[i] = null;
			}
		}
	}

	internal void stopallsfx()
	{
		for (int i = 0; i < sounds.Length; i++)
		{
			if (sounds[i] != null)
			{
				sounds[i].Stop(AudioStopOptions.AsAuthored);
			}
		}
	}

	internal void stopsfx(string soundname)
	{
		for (int i = 0; i < sounds.Length; i++)
		{
			if (sounds[i] != null && sounds[i].Name == soundname)
			{
				if (sounds[i].IsPlaying)
				{
					sounds[i].Stop(AudioStopOptions.AsAuthored);
				}
				break;
			}
		}
	}
}
