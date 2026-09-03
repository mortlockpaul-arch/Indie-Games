using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Core;

public static class Sound
{
	public enum MUSIC_STATE
	{
		PLAYING,
		STOPPED,
		PAUSED,
		FADE_IN,
		FADE_OUT
	}

	public static float MUSIC_VOL_DEC_MULTI = 1f;

	public static MUSIC_STATE m_music_state = MUSIC_STATE.STOPPED;

	public static string m_current_music = "";

	public static bool m_play_door_sound = true;

	public static SoundEffect m_door_open = null;

	public static float m_music_vol = 0f;

	public static void PlayMusic(Game game, Song music)
	{
		try
		{
			if (music == null || game == null)
			{
				return;
			}
			if (game.m_game_data != null)
			{
				if (game.m_game_data.GetState("Music") == m_current_music)
				{
					return;
				}
				m_current_music = game.m_game_data.GetState("Music");
			}
			m_music_state = MUSIC_STATE.PLAYING;
			MediaPlayer.IsRepeating = true;
			m_music_vol = game.m_game_settings.m_sound_volume * 0.1f * MUSIC_VOL_DEC_MULTI;
			MediaPlayer.Volume = m_music_vol;
			MediaPlayer.Play(music);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public static void FadeOutMusic(Game game)
	{
		try
		{
			if (!(m_current_music == ""))
			{
				m_music_state = MUSIC_STATE.FADE_OUT;
				m_music_vol = game.m_game_settings.m_sound_volume * 0.1f * MUSIC_VOL_DEC_MULTI;
				MediaPlayer.Volume = m_music_vol;
				if (game.m_game_data != null)
				{
					game.m_game_data.SetState("Music", "");
					m_current_music = "";
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public static void FadeInMusic()
	{
		try
		{
			m_music_state = MUSIC_STATE.FADE_IN;
			m_music_vol = 0f;
			MediaPlayer.Volume = m_music_vol;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public static void StopMusic()
	{
		try
		{
			MediaPlayer.Pause();
			m_current_music = "";
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public static void PauseMusic()
	{
		try
		{
			MediaPlayer.Pause();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public static void ResumeMusic()
	{
		try
		{
			MediaPlayer.Resume();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public static void Update(Game game, GameTime gameTime)
	{
		try
		{
			switch (m_music_state)
			{
			case MUSIC_STATE.FADE_IN:
				m_music_vol += (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f * 0.2f;
				if (m_music_vol >= game.m_game_settings.m_sound_volume * 0.1f * MUSIC_VOL_DEC_MULTI)
				{
					m_music_vol = game.m_game_settings.m_sound_volume * 0.1f * MUSIC_VOL_DEC_MULTI;
					m_music_state = MUSIC_STATE.PLAYING;
				}
				MediaPlayer.Volume = m_music_vol;
				break;
			case MUSIC_STATE.FADE_OUT:
				m_music_vol -= (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f * 0.2f;
				if (m_music_vol <= 0f)
				{
					m_music_vol = 0f;
					MediaPlayer.Pause();
				}
				MediaPlayer.Volume = m_music_vol;
				break;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
