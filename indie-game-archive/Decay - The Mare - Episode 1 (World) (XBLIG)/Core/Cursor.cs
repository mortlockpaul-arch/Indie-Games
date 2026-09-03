using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Core;

public class Cursor
{
	public enum CURSOR_STATE
	{
		IDLE,
		OVER_ANIM,
		OVER,
		OUT_ANIM
	}

	public CURSOR_STATE m_state;

	public Trigger.TRIGGER_TYPE m_trigger_type;

	protected Game m_game;

	protected Animation2D m_over_anim;

	protected Animation2D m_out_anim;

	protected Animation2D m_arrow_anim;

	protected Animation2D m_hand_anim;

	protected Animation2D m_magnifier_anim;

	protected Texture2D m_arrow_texture;

	protected Texture2D m_hand_texture;

	protected Texture2D m_magnifier_texture;

	protected Texture2D m_idle_texture;

	public Texture2D m_over_texture;

	public Vector2 m_pos = Vector2.Zero;

	protected float m_speed;

	protected float m_default_speed = 8f;

	protected float m_slow_speed = 3f;

	protected float m_inc_speed_multiplier = 1f;

	protected float m_dec_speed_multiplier = 0.8f;

	protected Color m_color = Color.White;

	protected bool m_slow_mode;

	public bool m_visible = true;

	public bool m_force_speed;

	public float m_custom_speed = 3f;

	public Cursor(Game game, Color color)
	{
		m_game = game;
		m_arrow_texture = m_game.Content.Load<Texture2D>("Cursor/Animations/Arrow/0001");
		m_hand_texture = m_game.Content.Load<Texture2D>("Cursor/Animations/Hand/0001");
		m_magnifier_texture = m_game.Content.Load<Texture2D>("Cursor/Animations/Magnifier/0001");
		m_idle_texture = m_arrow_texture;
		m_arrow_anim = new TextureAnimation(m_game, m_game.Content, "Cursor/Animations/Arrow/", 9u, reverse: false);
		m_arrow_anim.SetFPS(30.0);
		m_hand_anim = new TextureAnimation(m_game, m_game.Content, "Cursor/Animations/Hand/", 9u, reverse: false);
		m_hand_anim.SetFPS(30.0);
		m_magnifier_anim = new TextureAnimation(m_game, m_game.Content, "Cursor/Animations/Magnifier/", 9u, reverse: false);
		m_magnifier_anim.SetFPS(30.0);
		m_pos.X = Game.VIEW_RECT.Width / 2;
		m_pos.Y = Game.VIEW_RECT.Height / 2;
		m_color = color;
	}

	public virtual void Clear()
	{
		m_game = null;
		if (m_arrow_anim != null)
		{
			m_arrow_anim.Clear();
			m_arrow_anim = null;
		}
		if (m_hand_anim != null)
		{
			m_hand_anim.Clear();
			m_hand_anim = null;
		}
		if (m_magnifier_anim != null)
		{
			m_magnifier_anim.Clear();
			m_magnifier_anim = null;
		}
		if (m_over_anim != null)
		{
			m_over_anim.Clear();
			m_over_anim = null;
		}
		if (m_out_anim != null)
		{
			m_out_anim.Clear();
			m_out_anim = null;
		}
		m_arrow_texture = null;
		m_hand_texture = null;
		m_magnifier_texture = null;
		m_idle_texture = null;
		m_over_texture = null;
	}

	public void onOver(Trigger.TRIGGER_TYPE type)
	{
		if (m_game.m_update_cursor)
		{
			m_over_anim = null;
			m_trigger_type = type;
			switch (type)
			{
			case Trigger.TRIGGER_TYPE.ZOOM:
				m_over_anim = m_magnifier_anim;
				m_over_anim.m_reverse = true;
				m_over_anim.Play();
				m_over_texture = m_magnifier_texture;
				break;
			case Trigger.TRIGGER_TYPE.ZOOM_SMALL:
				m_over_anim = m_magnifier_anim;
				m_over_anim.m_reverse = true;
				m_over_anim.Play();
				m_over_texture = m_magnifier_texture;
				m_slow_mode = true;
				break;
			case Trigger.TRIGGER_TYPE.USE:
				m_over_anim = m_hand_anim;
				m_over_anim.m_reverse = true;
				m_over_anim.Play();
				m_over_texture = m_hand_texture;
				break;
			case Trigger.TRIGGER_TYPE.USE_SMALL:
				m_over_anim = m_hand_anim;
				m_over_anim.m_reverse = true;
				m_over_anim.Play();
				m_over_texture = m_hand_texture;
				m_slow_mode = true;
				break;
			}
			if (m_over_anim != null)
			{
				m_state = CURSOR_STATE.OVER_ANIM;
				m_arrow_anim.m_reverse = false;
				m_arrow_anim.Play();
			}
			else
			{
				m_state = CURSOR_STATE.OVER;
			}
		}
	}

	public void onOut()
	{
		if (m_game.m_update_cursor)
		{
			m_over_anim = null;
			m_slow_mode = false;
			if (m_over_texture == m_magnifier_texture)
			{
				m_out_anim = m_magnifier_anim;
				m_out_anim.m_reverse = false;
				m_out_anim.Play();
				m_over_texture = null;
			}
			else if (m_over_texture == m_hand_texture)
			{
				m_out_anim = m_hand_anim;
				m_out_anim.m_reverse = false;
				m_out_anim.Play();
				m_over_texture = null;
			}
			if (m_out_anim != null)
			{
				m_state = CURSOR_STATE.OUT_ANIM;
				m_arrow_anim.m_reverse = true;
				m_arrow_anim.Play();
			}
			else
			{
				m_state = CURSOR_STATE.IDLE;
			}
		}
	}

	public virtual void Update(TimeSpan elapsed)
	{
		if (m_state == CURSOR_STATE.OVER_ANIM && m_over_anim != null)
		{
			if (m_arrow_anim.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_over_anim.Update(elapsed);
				if (m_over_anim.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
				{
					m_state = CURSOR_STATE.OVER;
				}
			}
			else
			{
				m_arrow_anim.Update(elapsed);
			}
		}
		if (m_state == CURSOR_STATE.OUT_ANIM && m_out_anim != null)
		{
			if (m_out_anim.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_arrow_anim.Update(elapsed);
				if (m_arrow_anim.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
				{
					m_state = CURSOR_STATE.IDLE;
				}
			}
			else
			{
				m_out_anim.Update(elapsed);
			}
		}
		KeyboardState state = Keyboard.GetState();
		if (m_slow_mode)
		{
			m_speed = m_slow_speed;
			m_speed += GamePad.GetState(Game.PLAYER_INDEX).Triggers.Right * m_slow_speed * m_inc_speed_multiplier;
			m_speed -= GamePad.GetState(Game.PLAYER_INDEX).Triggers.Left * m_slow_speed * m_dec_speed_multiplier;
		}
		else
		{
			m_speed = m_default_speed;
			m_speed += GamePad.GetState(Game.PLAYER_INDEX).Triggers.Right * m_default_speed * m_inc_speed_multiplier;
			m_speed -= GamePad.GetState(Game.PLAYER_INDEX).Triggers.Left * m_default_speed * m_dec_speed_multiplier;
		}
		if (m_force_speed)
		{
			m_speed = m_custom_speed;
		}
		m_pos.X += GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X * m_speed;
		m_pos.Y -= GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.Y * m_speed;
		if (state.IsKeyDown(Keys.Left))
		{
			if (state.IsKeyDown(Keys.LeftShift))
			{
				m_pos.X -= m_slow_speed;
			}
			else
			{
				m_pos.X -= m_default_speed;
			}
		}
		if (state.IsKeyDown(Keys.Right))
		{
			if (state.IsKeyDown(Keys.LeftShift))
			{
				m_pos.X += m_slow_speed;
			}
			else
			{
				m_pos.X += m_default_speed;
			}
		}
		if (state.IsKeyDown(Keys.Up))
		{
			if (state.IsKeyDown(Keys.LeftShift))
			{
				m_pos.Y -= m_slow_speed;
			}
			else
			{
				m_pos.Y -= m_default_speed;
			}
		}
		if (state.IsKeyDown(Keys.Down))
		{
			if (state.IsKeyDown(Keys.LeftShift))
			{
				m_pos.Y += m_slow_speed;
			}
			else
			{
				m_pos.Y += m_default_speed;
			}
		}
		if (m_pos.X < 2f)
		{
			m_pos.X = 2f;
			m_speed = 0f;
		}
		if (m_pos.Y < 1f)
		{
			m_pos.Y = 1f;
			m_speed = 0f;
		}
		if (m_pos.X > (float)(Game.VIEW_RECT.Width - 25))
		{
			m_pos.X = Game.VIEW_RECT.Width - 25;
			m_speed = 0f;
		}
		if (m_pos.Y > (float)(Game.VIEW_RECT.Height - 34))
		{
			m_pos.Y = Game.VIEW_RECT.Height - 34;
			m_speed = 0f;
		}
	}

	protected void DrawCursor(SpriteBatch SB, Vector2 pos, int size, Color color)
	{
		try
		{
			switch (m_state)
			{
			case CURSOR_STATE.IDLE:
				if (m_idle_texture != null)
				{
					SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
					SB.Draw(m_idle_texture, new Rectangle((int)pos.X - 6, (int)pos.Y - 3, size, size), new Rectangle(0, 0, 64, 64), color);
					SB.End();
				}
				break;
			case CURSOR_STATE.OVER:
				if (m_over_texture != null)
				{
					SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
					SB.Draw(m_over_texture, new Rectangle((int)pos.X - 6, (int)pos.Y - 3, size, size), new Rectangle(0, 0, 64, 64), color);
					SB.End();
				}
				break;
			case CURSOR_STATE.OVER_ANIM:
				if (m_arrow_anim.m_state != Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
				{
					m_arrow_anim.Draw(SB, new Rectangle((int)pos.X - 6, (int)pos.Y - 3, size, size), new Rectangle(0, 0, 64, 64), color);
				}
				else if (m_over_anim != null)
				{
					m_over_anim.Draw(SB, new Rectangle((int)pos.X - 6, (int)pos.Y - 3, size, size), new Rectangle(0, 0, 64, 64), color);
				}
				break;
			case CURSOR_STATE.OUT_ANIM:
				if (m_out_anim.m_state != Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
				{
					m_out_anim.Draw(SB, new Rectangle((int)pos.X - 6, (int)pos.Y - 3, size, size), new Rectangle(0, 0, 64, 64), color);
				}
				else if (m_arrow_anim.m_state != Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
				{
					m_arrow_anim.Draw(SB, new Rectangle((int)pos.X - 6, (int)pos.Y - 3, size, size), new Rectangle(0, 0, 64, 64), color);
				}
				break;
			}
		}
		catch
		{
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
		try
		{
			if (SB != null && m_visible)
			{
				DrawCursor(SB, m_pos, 60, m_color);
			}
		}
		catch
		{
		}
	}
}
