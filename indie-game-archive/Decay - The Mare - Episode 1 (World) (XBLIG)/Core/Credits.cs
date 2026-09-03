using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SGSCore;

namespace Core;

public class Credits
{
	protected enum TEXT_STATE
	{
		WAIT,
		FADE_IN,
		SHOW,
		FADE_OUT
	}

	protected Game m_game;

	protected SpriteFont m_font;

	protected SpriteFont m_font2;

	protected float m_text_alpha;

	protected int m_text_index;

	protected int m_last_index;

	protected TEXT_STATE m_text_state;

	protected float m_timer;

	public Credits(Game game, SGSContentLoader CL)
	{
		m_game = game;
		m_font = CL.LoadFont("Fonts/SpriteFont2");
		m_font2 = CL.LoadFont("Fonts/SpriteFont1");
	}

	public virtual void Clear()
	{
		m_game = null;
		m_font = null;
		m_font2 = null;
	}

	public virtual void Reset()
	{
		m_text_index = 0;
		m_timer = 0f;
		m_text_state = TEXT_STATE.WAIT;
		m_text_alpha = 0f;
		m_game.m_a_pressed = true;
		m_game.m_b_pressed = true;
	}

	protected virtual void onClose()
	{
		try
		{
			m_game.onCreditsClosed();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual void Update(TimeSpan elapsed)
	{
		KeyboardState state = Keyboard.GetState();
		if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.B == ButtonState.Pressed || state.IsKeyDown(Keys.B) || GamePad.GetState(Game.PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.A) || GamePad.GetState(Game.PLAYER_INDEX).Buttons.Start == ButtonState.Pressed || state.IsKeyDown(Keys.Space))
		{
			if (!m_game.m_b_pressed)
			{
				m_game.m_b_pressed = true;
				onClose();
			}
		}
		else
		{
			m_game.m_b_pressed = false;
		}
		switch (m_text_state)
		{
		case TEXT_STATE.WAIT:
			m_timer -= (float)elapsed.TotalSeconds;
			if (m_timer <= 0f)
			{
				m_text_state = TEXT_STATE.FADE_IN;
			}
			break;
		case TEXT_STATE.FADE_IN:
			m_text_alpha += (float)elapsed.TotalSeconds * 0.5f;
			if (m_text_alpha >= 1f)
			{
				m_text_alpha = 1f;
				m_text_state = TEXT_STATE.SHOW;
				m_timer = 5f;
			}
			break;
		case TEXT_STATE.SHOW:
			m_timer -= (float)elapsed.TotalSeconds;
			if (m_timer <= 0f)
			{
				m_text_state = TEXT_STATE.FADE_OUT;
			}
			break;
		case TEXT_STATE.FADE_OUT:
			m_text_alpha -= (float)elapsed.TotalSeconds * 0.5f;
			if (m_text_alpha <= 0f)
			{
				m_text_state = TEXT_STATE.FADE_IN;
				m_text_index++;
				if (m_text_index > m_last_index)
				{
					m_text_index = 0;
				}
			}
			break;
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
	}
}
