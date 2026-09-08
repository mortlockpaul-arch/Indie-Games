using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Quasar.Global;

namespace Quasar.GameUtils.Audio;

public class Jukebox
{
	public const string MUSIC_DIR = "Music/";

	public const string DEFAULT_PREFIX = "Game_";

	private static Jukebox instance;

	private Song currentSong;

	private List<string> playList = new List<string>();

	private int currentSongIndex = -1;

	private int lastSongIndex = -1;

	private bool repeat;

	private bool randomize = true;

	private float volume = 1f;

	private long lastPlayTime = -1L;

	private bool enabled;

	private static Jukebox activeJukebox;

	public static Jukebox Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new Jukebox();
			}
			return instance;
		}
	}

	protected string CurrentSongFile => playList[currentSongIndex];

	public bool Repeat
	{
		get
		{
			return repeat;
		}
		set
		{
			repeat = value;
		}
	}

	public bool Randomize
	{
		get
		{
			return randomize;
		}
		set
		{
			randomize = value;
		}
	}

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
				SetVolume();
			}
		}
	}

	public static Jukebox ActiveJukebox => activeJukebox;

	public bool Playing
	{
		get
		{
			if (MediaPlayer.State == MediaState.Playing)
			{
				return activeJukebox == this;
			}
			return false;
		}
	}

	private void SetVolume()
	{
		if (MediaPlayer.GameHasControl)
		{
			MediaPlayer.Volume = volume * Quasar.Audio.MusicVolume;
		}
	}

	private void MediaPlayer_Changed(object sender, EventArgs e)
	{
		if ((lastPlayTime == -1 || Timer.DefaultTimer.TimeSince(lastPlayTime) > 5000) && MediaPlayer.GameHasControl)
		{
			SetNextSong();
		}
	}

	public void AddMusicFile(string filename)
	{
		playList.Add(filename);
	}

	public void AddMusicDir(string path, string prefix)
	{
		DirectoryManager.FileInfo[] files = DirectoryManager.GetFiles(path, recursive: false);
		DirectoryManager.FileInfo[] array = files;
		foreach (DirectoryManager.FileInfo fileInfo in array)
		{
			string item = path + fileInfo.Name;
			if (fileInfo.Name.StartsWith(prefix))
			{
				playList.Add(item);
			}
		}
	}

	public Jukebox()
		: this("Game_")
	{
	}

	public Jukebox(string prefix)
	{
		AddMusicDir("Music/", prefix);
	}

	public void Disable()
	{
		if (enabled)
		{
			MediaPlayer.MediaStateChanged -= MediaPlayer_Changed;
			enabled = false;
			activeJukebox = null;
		}
	}

	public void Enable()
	{
		if (activeJukebox != null && activeJukebox != this)
		{
			MediaPlayer.MediaStateChanged -= activeJukebox.MediaPlayer_Changed;
		}
		MediaPlayer.MediaStateChanged -= MediaPlayer_Changed;
		MediaPlayer.MediaStateChanged += MediaPlayer_Changed;
		enabled = true;
		activeJukebox = this;
	}

	public void Start()
	{
		if (activeJukebox != this)
		{
			Enable();
		}
		if (playList.Count > 0)
		{
			SetNextSong();
		}
	}

	public void SetNextSong()
	{
		currentSongIndex = GetNextSong();
		PlaySong();
	}

	public void SetNextSong(string songName, bool restartIfPlaying)
	{
		int num = playList.IndexOf(songName);
		if (restartIfPlaying || currentSongIndex != num)
		{
			currentSongIndex = num;
			PlaySong();
		}
	}

	public void SetNextSong(int nextSongIndex)
	{
		currentSongIndex = nextSongIndex;
		PlaySong();
	}

	private void PlaySong()
	{
		if (currentSongIndex == -1)
		{
			return;
		}
		try
		{
			currentSong = Engine.ContentManager.Load<Song>(CurrentSongFile);
			lastPlayTime = Timer.DefaultTimer.TotalTime;
			JukeboxManager.Instance.ChangeSong(currentSong);
		}
		catch (Exception)
		{
			currentSongIndex = -1;
		}
	}

	protected int GetNextSong()
	{
		if (playList.Count == 0)
		{
			return -1;
		}
		if (currentSongIndex == -1)
		{
			currentSongIndex = lastSongIndex;
			lastSongIndex = -1;
		}
		if (currentSongIndex == -1)
		{
			if (randomize)
			{
				return GameMath.Random.Next(playList.Count);
			}
			return 0;
		}
		if (repeat)
		{
			return currentSongIndex;
		}
		if (randomize)
		{
			if (playList.Count == 1)
			{
				return 0;
			}
			int num = GameMath.Random.Next(playList.Count - 1);
			if (num >= currentSongIndex)
			{
				num++;
			}
			return num;
		}
		return (currentSongIndex + 1) % playList.Count;
	}

	public void Stop()
	{
		Disable();
		if (currentSongIndex != -1)
		{
			if (MediaPlayer.GameHasControl)
			{
				MediaPlayer.Stop();
			}
			lastSongIndex = currentSongIndex;
			currentSongIndex = -1;
		}
	}
}
