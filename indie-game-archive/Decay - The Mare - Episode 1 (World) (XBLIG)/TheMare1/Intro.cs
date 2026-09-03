using System;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SGSCore;

namespace TheMare1;

public class Intro : Core.Intro
{
	private enum INTRO_STATE
	{
		WAIT,
		FADE_IN_LOGO,
		SHOW_LOGO,
		FADE_OUT_LOGO,
		SHOW_INTRO1
	}

	protected Texture2D m_logo;

	protected Movie m_movie;

	protected float m_alpha;

	private INTRO_STATE m_state;

	protected float m_timer;

	protected string m_text = "";

	public Intro(TheMare1 game, SGSContentLoader CL)
		: base(game)
	{
		m_game = game;
		m_logo = CL.LoadTexture("Intro/sg_logo");
		m_movie = new Movie();
		m_movie.Load(CL, "Intro/intro1");
	}

	public override void Start()
	{
		m_state = INTRO_STATE.WAIT;
		m_timer = 1f;
		m_alpha = 0f;
		m_game.m_a_pressed = true;
	}

	public override void Clear()
	{
		m_game = null;
		m_logo = null;
		if (m_movie != null)
		{
			m_movie.Clear();
			m_movie = null;
		}
		base.Clear();
	}

	public override void Update(TimeSpan elapsed)
	{
		KeyboardState state = Keyboard.GetState();
		if (GamePad.GetState(Core.Game.PLAYER_INDEX).Buttons.A == ButtonState.Pressed || GamePad.GetState(Core.Game.PLAYER_INDEX).Buttons.Start == ButtonState.Pressed || state.IsKeyDown(Keys.A))
		{
			if (!m_game.m_a_pressed)
			{
				m_game.m_a_pressed = true;
				if (m_state == INTRO_STATE.FADE_IN_LOGO || m_state == INTRO_STATE.SHOW_LOGO || m_state == INTRO_STATE.FADE_OUT_LOGO)
				{
					m_alpha = 0f;
					m_state = INTRO_STATE.SHOW_INTRO1;
					m_movie.Play();
					m_movie.m_video_player.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.3f;
				}
				else
				{
					m_game.onIntroFinished();
				}
				return;
			}
		}
		else
		{
			m_game.m_a_pressed = false;
		}
		switch (m_state)
		{
		case INTRO_STATE.WAIT:
			m_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_timer <= 0f)
			{
				m_timer = 0f;
				m_state = INTRO_STATE.FADE_IN_LOGO;
			}
			break;
		case INTRO_STATE.FADE_IN_LOGO:
			m_alpha += (float)elapsed.TotalSeconds * 1f;
			if (m_alpha >= 1f)
			{
				m_alpha = 1f;
				m_state = INTRO_STATE.SHOW_LOGO;
				m_timer = 3f;
			}
			break;
		case INTRO_STATE.SHOW_LOGO:
			m_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_timer <= 0f)
			{
				m_timer = 0f;
				m_state = INTRO_STATE.FADE_OUT_LOGO;
			}
			break;
		case INTRO_STATE.FADE_OUT_LOGO:
			m_alpha -= (float)elapsed.TotalSeconds * 0.5f;
			if (m_alpha <= 0f)
			{
				m_alpha = 0f;
				m_state = INTRO_STATE.SHOW_INTRO1;
				m_movie.Play();
				m_movie.m_video_player.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.3f;
			}
			break;
		case INTRO_STATE.SHOW_INTRO1:
			if (m_movie.m_video_player.State == MediaState.Stopped)
			{
				m_game.onIntroFinished();
			}
			break;
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		switch (m_state)
		{
		case INTRO_STATE.WAIT:
			break;
		case INTRO_STATE.FADE_IN_LOGO:
		case INTRO_STATE.SHOW_LOGO:
		case INTRO_STATE.FADE_OUT_LOGO:
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_logo, Core.Game.VIEW_RECT, Color.White * m_alpha);
			SB.End();
			break;
		default:
			if (m_movie != null && m_movie.m_video_player.State == MediaState.Playing)
			{
				m_movie.Draw(SB, Color.White);
			}
			break;
		}
	}
}
