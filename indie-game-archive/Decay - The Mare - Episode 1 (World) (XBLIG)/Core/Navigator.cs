using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public class Navigator
{
	private enum NAVIGATOR_STATE
	{
		IDLE,
		FADE_OUT,
		FADE_IN
	}

	private NAVIGATOR_STATE m_state;

	private Texture2D m_background;

	private Texture2D m_left;

	private Texture2D m_right;

	private Texture2D m_up;

	private Texture2D m_down;

	private Vector2 m_pos;

	public bool m_left_enabled;

	public bool m_right_enabled;

	public bool m_up_enabled;

	public bool m_down_enabled;

	private Game m_game;

	public Navigator(Game game)
	{
		m_game = game;
		m_background = m_game.Content.Load<Texture2D>("HUD/Navigator/arrow_bg");
		m_left = m_game.Content.Load<Texture2D>("HUD/Navigator/arrow_west");
		m_right = m_game.Content.Load<Texture2D>("HUD/Navigator/arrow_east");
		m_up = m_game.Content.Load<Texture2D>("HUD/Navigator/arrow_north");
		m_down = m_game.Content.Load<Texture2D>("HUD/Navigator/arrow_south");
		m_pos = new Vector2(Game.TS_AREA.Right - m_background.Width, Game.TS_AREA.Bottom - m_background.Height);
	}

	public virtual void Clear()
	{
		m_game = null;
		m_background = null;
		m_left = null;
		m_right = null;
		m_up = null;
		m_down = null;
	}

	public virtual void Setup(bool left, bool right, bool up, bool down)
	{
		m_left_enabled = left;
		m_right_enabled = right;
		m_up_enabled = up;
		m_down_enabled = down;
	}

	public void FadeOut()
	{
		if (m_state != NAVIGATOR_STATE.FADE_OUT)
		{
			m_game.m_hud.m_alpha = 1f;
			m_state = NAVIGATOR_STATE.FADE_OUT;
		}
	}

	public void FadeIn()
	{
		m_game.m_hud.m_alpha = 0f;
		m_state = NAVIGATOR_STATE.FADE_IN;
	}

	public virtual void Update(TimeSpan elapsed)
	{
		switch (m_state)
		{
		case NAVIGATOR_STATE.FADE_OUT:
			m_game.m_hud.m_alpha -= (float)elapsed.TotalSeconds * 2f;
			if (m_game.m_hud.m_alpha <= 0f)
			{
				m_game.m_hud.m_alpha = 0f;
				m_state = NAVIGATOR_STATE.IDLE;
			}
			break;
		case NAVIGATOR_STATE.FADE_IN:
			m_game.m_hud.m_alpha += (float)elapsed.TotalSeconds * 1f;
			if (m_game.m_hud.m_alpha >= 1f)
			{
				m_game.m_hud.m_alpha = 1f;
				m_state = NAVIGATOR_STATE.IDLE;
			}
			break;
		case NAVIGATOR_STATE.IDLE:
			break;
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
		SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		SB.Draw(m_background, m_pos, m_game.m_hud.m_color * m_game.m_hud.m_alpha);
		if (m_left_enabled)
		{
			SB.Draw(m_left, m_pos, m_game.m_hud.m_color * m_game.m_hud.m_alpha);
		}
		if (m_right_enabled)
		{
			SB.Draw(m_right, m_pos, m_game.m_hud.m_color * m_game.m_hud.m_alpha);
		}
		if (m_up_enabled)
		{
			SB.Draw(m_up, m_pos, m_game.m_hud.m_color * m_game.m_hud.m_alpha);
		}
		if (m_down_enabled)
		{
			SB.Draw(m_down, m_pos, m_game.m_hud.m_color * m_game.m_hud.m_alpha);
		}
		SB.End();
	}
}
