using System;
using Core;
using Core.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TheMare1.World.Views.DreamOwnRoom;

internal class PlateZoom : View
{
	private enum STATE
	{
		WAIT,
		FADE_IN_TEXT,
		SHOW_TEXT,
		FADE_OUT_TEXT,
		SHOW_MOVIE,
		FADE_OUT
	}

	private STATE m_state;

	protected float m_alpha;

	private SpriteFont m_font;

	protected float m_timer;

	protected string m_text = "";

	protected SoundEffect m_what_is_a_friend;

	protected SoundEffectInstance m_what_is_a_friend_inst;

	protected Movie m_movie;

	public PlateZoom(Core.Game game, Area room, string xml_path)
		: base(game, room, xml_path)
	{
		m_state = STATE.WAIT;
		m_font = getContentLoader().LoadFont("Fonts/SpriteFont2");
		m_what_is_a_friend = getContentLoader().LoadSound("Intro/What_is_a_friend");
		m_what_is_a_friend_inst = m_what_is_a_friend.CreateInstance();
		m_movie = new Movie();
		m_movie.Load(getContentLoader(), "Intro/intro2");
	}

	public override void Clear()
	{
		m_font = null;
		if (m_what_is_a_friend_inst != null)
		{
			m_what_is_a_friend_inst.Dispose();
			m_what_is_a_friend_inst = null;
		}
		if (m_what_is_a_friend != null)
		{
			m_what_is_a_friend.Dispose();
			m_what_is_a_friend = null;
		}
		if (m_movie != null)
		{
			m_movie.Clear();
			m_movie = null;
		}
		base.Clear();
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "PlateZoom.onTakeMedicine2")
		{
			m_game.m_game_menu_enabled = false;
			m_game.m_a_pressed = true;
			m_state = STATE.FADE_IN_TEXT;
			m_alpha = 0f;
			m_text = m_game.m_language.GetString("What is a friend?");
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		try
		{
			base.Update(elapsed);
			if (m_state == STATE.WAIT)
			{
				return;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.A) || GamePad.GetState(Core.Game.PLAYER_INDEX).IsButtonDown(Buttons.A))
			{
				if (!m_game.m_a_pressed)
				{
					m_game.m_a_pressed = true;
					if (m_state == STATE.FADE_IN_TEXT || m_state == STATE.SHOW_TEXT || m_state == STATE.FADE_OUT_TEXT)
					{
						if (m_what_is_a_friend_inst.State == SoundState.Playing)
						{
							m_what_is_a_friend_inst.Stop();
						}
						m_alpha = 1f;
						m_state = STATE.SHOW_MOVIE;
						m_movie.Play();
						m_movie.m_video_player.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.3f;
						return;
					}
					if (m_state == STATE.SHOW_MOVIE)
					{
						m_timer = 0f;
						m_alpha = 1f;
						m_state = STATE.FADE_OUT;
						return;
					}
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			switch (m_state)
			{
			case STATE.FADE_IN_TEXT:
				m_alpha += (float)elapsed.TotalSeconds * 1f;
				if (m_alpha >= 1f)
				{
					m_alpha = 1f;
					m_state = STATE.SHOW_TEXT;
					m_timer = 5f;
					m_what_is_a_friend_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f;
					m_what_is_a_friend_inst.Play();
				}
				break;
			case STATE.SHOW_TEXT:
				m_timer -= (float)elapsed.TotalSeconds;
				if (m_timer <= 0f)
				{
					m_timer = 0f;
					m_state = STATE.FADE_OUT_TEXT;
				}
				break;
			case STATE.FADE_OUT_TEXT:
				m_alpha -= (float)elapsed.TotalSeconds * 1f;
				if (m_alpha <= 0f)
				{
					m_alpha = 1f;
					m_state = STATE.SHOW_MOVIE;
					m_movie.Play();
					m_movie.m_video_player.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.3f;
				}
				break;
			case STATE.SHOW_MOVIE:
				if ((m_movie.m_video.Duration - m_movie.m_video_player.PlayPosition).TotalMilliseconds <= 3000.0)
				{
					m_timer = 0f;
					m_alpha = 1f;
					m_state = STATE.FADE_OUT;
				}
				break;
			case STATE.FADE_OUT:
			{
				m_alpha -= (float)elapsed.TotalSeconds * 0.33f;
				if (m_alpha <= 0f)
				{
					m_alpha = 0f;
				}
				float volume = m_movie.m_video_player.Volume;
				volume -= (float)elapsed.TotalMilliseconds * 0.001f * 0.15f;
				if (volume < 0f)
				{
					volume = 0f;
				}
				m_movie.m_video_player.Volume = volume;
				if (m_movie != null && (m_movie.m_video_player.State == MediaState.Stopped || m_alpha == 0f))
				{
					m_movie.m_video_player.Stop();
					onMovieFinished();
				}
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	private void onMovieFinished()
	{
		try
		{
			m_game.m_game_menu_enabled = true;
			m_state = STATE.WAIT;
			m_game.HandleEvent("PlateZoom.onWakeUp");
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		base.Draw(SB);
		switch (m_state)
		{
		case STATE.SHOW_MOVIE:
		case STATE.FADE_OUT:
			if (m_movie != null && m_movie.m_video_player.State == MediaState.Playing)
			{
				m_movie.Draw(SB, Color.White * m_alpha);
			}
			break;
		case STATE.FADE_IN_TEXT:
		case STATE.SHOW_TEXT:
		case STATE.FADE_OUT_TEXT:
			if (m_text != "")
			{
				Vector2 vector = m_font.MeasureString(m_text);
				Vector2 zero = Vector2.Zero;
				zero.X = ((float)Core.Game.VIEW_RECT.Width - vector.X) / 2f;
				zero.Y = ((float)Core.Game.VIEW_RECT.Height - vector.Y) / 2f;
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.DrawString(m_font, m_text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black * m_alpha);
				SB.DrawString(m_font, m_text, zero, Color.White * m_alpha);
				SB.End();
			}
			break;
		}
	}
}
