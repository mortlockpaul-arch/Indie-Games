using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Eyehook.Framework;

public class AudioManager
{
	private static AudioManager audioManager;

	public AudioEngine audioEngine;

	public WaveBank waveBank;

	public SoundBank soundBank;

	public AudioCategory musicCategory;

	public AudioCategory effectsCategory;

	private float musicVolume = 1f;

	private float effectsVolume = 1f;

	private bool muted;

	public static AudioManager instance
	{
		get
		{
			if (audioManager == null)
			{
				audioManager = new AudioManager();
			}
			return audioManager;
		}
	}

	private AudioManager()
	{
	}

	public void Initialize()
	{
		audioEngine = new AudioEngine("Content\\Audio\\" + MC.Game.Name + ".xgs");
		waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
		soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");
		musicCategory = audioEngine.GetCategory("Music");
		effectsCategory = audioEngine.GetCategory("Effects");
	}

	public void unloadContent()
	{
		soundBank.Dispose();
		waveBank.Dispose();
		audioEngine.Dispose();
	}

	public void playCue(string cueName)
	{
		soundBank.PlayCue(cueName);
	}

	public Cue getCue(string cueName)
	{
		return soundBank.GetCue(cueName);
	}

	public void setEffectsVolume(float volume)
	{
		effectsVolume = volume;
		effectsCategory.SetVolume(effectsVolume);
	}

	public void setMusicVolume(float volume)
	{
		musicVolume = volume;
		musicCategory.SetVolume(musicVolume);
	}

	public void PauseMusic()
	{
		musicCategory.Pause();
	}

	public void ResumeMusic()
	{
		musicCategory.Resume();
	}

	public void muteMusic()
	{
		if (!muted)
		{
			musicCategory.SetVolume(0f);
			muted = true;
		}
	}

	public void unMuteMusic()
	{
		if (muted)
		{
			musicCategory.SetVolume(musicVolume);
			muted = false;
		}
	}

	public void update(GameTime gameTime)
	{
		audioEngine.Update();
	}
}
