using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Renderer;

namespace MusicPlayer;

public static class Mp3MusicPlayer
{
	private static Song sm_song;

	private static bool sm_bFadeIn;

	private static bool sm_bFadeOut;

	private static bool sm_bMusicQueued;

	private static string sm_filePlaying;

	private static float maxVol = 0.7f;

	private static bool sm_shouldReplay;

	private static bool m_bPaused;

	private static bool m_bGuidePause;

	private static int sm_secondaryOffset;

	private static int timerOffset;

	private static List<Song> sm_availableSongs;

	private static List<string> sm_availableSongNames;

	private static int m_iFlip;

	private static float m_fFadeTime = 1f;

	public static int SecondaryOffset
	{
		get
		{
			return sm_secondaryOffset;
		}
		set
		{
			sm_secondaryOffset = value;
		}
	}

	public static float Volume
	{
		get
		{
			return maxVol;
		}
		set
		{
			maxVol = value;
			if (!sm_bFadeIn && !sm_bFadeOut)
			{
				MediaPlayer.Volume = maxVol;
			}
		}
	}

	public static void PreLoadSong(string filename)
	{
		SceneRenderer.GetContentManager().Load<Song>(filename);
	}

	public static void Initialize(string filename, bool shouldReplay, bool forceReplay)
	{
		m_iFlip = 0;
		InitInternal(filename, shouldReplay, forceReplay);
		sm_availableSongs = null;
		sm_availableSongNames = null;
	}

	public static void Initialize(List<string> filenames, bool shouldReplay, bool forceReplay)
	{
		sm_availableSongNames = filenames;
		sm_availableSongs = new List<Song>();
		for (int i = 0; i < sm_availableSongNames.Count; i++)
		{
			sm_availableSongs.Add(SceneRenderer.GetContentManager().Load<Song>(sm_availableSongNames[i]));
		}
		InitInternal(sm_availableSongNames[(int)SceneRenderer.GetRand(0f, sm_availableSongNames.Count)], shouldReplay: true, forceReplay: true);
	}

	private static void InitInternal(string filename, bool shouldReplay, bool forceReplay)
	{
		timerOffset = 0;
		sm_shouldReplay = shouldReplay;
		if (sm_filePlaying == null || !sm_filePlaying.Equals(filename) || forceReplay)
		{
			sm_filePlaying = filename;
			sm_song = SceneRenderer.GetContentManager().Load<Song>(filename);
			Play(maxVol);
		}
		else if (MediaPlayer.Volume == 0f)
		{
			Play(maxVol);
		}
		sm_bFadeIn = false;
		sm_bFadeOut = false;
		m_bPaused = false;
		m_bGuidePause = false;
	}

	public static void Update(TimeTracker gameTime, bool guideVisible)
	{
		m_iFlip++;
		if (m_iFlip > 60)
		{
			m_iFlip -= 60;
			MediaState state = MediaPlayer.State;
			if (guideVisible)
			{
				if (!m_bPaused && state == MediaState.Playing)
				{
					MediaPlayer.Pause();
					m_bGuidePause = true;
				}
			}
			else if (m_bGuidePause)
			{
				MediaPlayer.Resume();
				m_bGuidePause = false;
			}
			if (sm_song != null && !sm_bMusicQueued && sm_shouldReplay && state == MediaState.Stopped)
			{
				if (sm_availableSongNames != null)
				{
					int index = (int)SceneRenderer.GetRand(0f, sm_availableSongNames.Count);
					sm_filePlaying = sm_availableSongNames[index];
					sm_song = sm_availableSongs[index];
				}
				Play(MediaPlayer.Volume);
			}
			else if (state == MediaState.Playing)
			{
				sm_bMusicQueued = false;
			}
		}
		if (sm_bFadeIn)
		{
			MediaPlayer.Volume += gameTime.FractionOfSecond / m_fFadeTime;
			if (MediaPlayer.Volume >= maxVol)
			{
				MediaPlayer.Volume = maxVol;
				sm_bFadeIn = false;
			}
		}
		if (sm_bFadeOut)
		{
			MediaPlayer.Volume -= gameTime.FractionOfSecond / m_fFadeTime;
			if (MediaPlayer.Volume <= 0f)
			{
				MediaPlayer.Volume = 0f;
				sm_bFadeOut = false;
			}
		}
	}

	private static void Play(float volume)
	{
		MediaPlayer.Volume = volume;
		sm_bFadeOut = false;
		MediaPlayer.Play(sm_song);
		sm_bMusicQueued = true;
		MediaPlayer.IsRepeating = false;
	}

	public static void Pause()
	{
		MediaPlayer.Pause();
		m_bPaused = true;
	}

	public static void Unpause()
	{
		MediaPlayer.Resume();
		m_bPaused = false;
	}

	public static void FadeOut(float fadeTime)
	{
		m_fFadeTime = fadeTime;
		sm_bFadeIn = false;
		sm_bFadeOut = true;
	}

	public static void FadeIn(float fadeTime)
	{
		m_fFadeTime = fadeTime;
		MediaPlayer.Volume = 0f;
		sm_bFadeIn = true;
		sm_bFadeOut = false;
	}
}
