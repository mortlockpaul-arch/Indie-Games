using System;
using Microsoft.Xna.Framework.Audio;

namespace Quasar.GameUtils.Audio;

public class XACTJukebox
{
	public const string MUSIC_DIR = "Content/Music/";

	private static XACTJukebox instance;

	private Cue currentSong;

	private AudioCategory? musicCategory = null;

	private AudioEngine audioEngine;

	private WaveBank waveBank;

	private SoundBank soundBank;

	private string currentSonglist = "";

	private float volume = 1f;

	public static XACTJukebox Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new XACTJukebox("Music");
			}
			return instance;
		}
	}

	public static bool HasInstance => instance != null;

	public string CurrentSongList => currentSonglist;

	public float Volume
	{
		get
		{
			return volume;
		}
		set
		{
			if (volume != value)
			{
				volume = value;
				UpdateVolume();
			}
		}
	}

	public void UpdateVolume()
	{
		if (musicCategory.HasValue)
		{
			musicCategory.Value.SetVolume(volume * Quasar.Audio.MusicVolume);
		}
	}

	public void Update()
	{
		if (audioEngine != null)
		{
			audioEngine.Update();
		}
	}

	protected XACTJukebox(string file)
	{
		try
		{
			audioEngine = new AudioEngine("Content/Music/" + file + ".xgs");
			soundBank = new SoundBank(audioEngine, "Content/Music/" + file + ".xsb");
			waveBank = new WaveBank(audioEngine, "Content/Music/" + file + ".xwb", 0, 4);
			musicCategory = audioEngine.GetCategory("Music");
			UpdateVolume();
			Update();
		}
		catch
		{
		}
	}

	public void StartSonglist(string name)
	{
		if (currentSonglist != name)
		{
			if (currentSong != null)
			{
				currentSong.Stop(AudioStopOptions.AsAuthored);
			}
			currentSonglist = name;
			try
			{
				currentSong = soundBank.GetCue(name);
				currentSong.Play();
				UpdateVolume();
			}
			catch (Exception)
			{
			}
		}
	}

	public void Pause()
	{
		if (currentSong != null)
		{
			currentSong.Pause();
		}
	}

	public void Resume()
	{
		if (currentSong != null && currentSong.IsPaused)
		{
			currentSong.Resume();
		}
	}

	public void Stop()
	{
		if (currentSong != null)
		{
			currentSong.Stop(AudioStopOptions.AsAuthored);
		}
		currentSonglist = "";
	}
}
