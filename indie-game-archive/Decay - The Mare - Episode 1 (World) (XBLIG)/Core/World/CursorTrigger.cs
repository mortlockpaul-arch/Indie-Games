using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Core.World;

public class CursorTrigger : Trigger
{
	public enum AREATRIGGER_TYPE
	{
		RECTANGLE,
		ALPHA
	}

	protected AREATRIGGER_TYPE m_area_type;

	protected Game m_game;

	protected Rectangle m_rect;

	public Texture2D m_texture;

	protected Trigger m_trigger;

	public Color[] m_pixel_data;

	protected string m_event = "";

	public CursorTrigger(Game game, Trigger trigger, TRIGGER_TYPE type)
		: base(type)
	{
		m_class_name = "CursorTrigger";
		m_game = game;
		m_trigger = trigger;
	}

	public CursorTrigger(Game game, Rectangle rect, Trigger trigger, TRIGGER_TYPE type)
		: this(game, trigger, type)
	{
		m_class_name = "CursorTrigger";
		m_rect = new Rectangle(rect.X, rect.Y, rect.Width - rect.X, rect.Height - rect.Y);
		m_area_type = AREATRIGGER_TYPE.RECTANGLE;
	}

	public CursorTrigger(Game game, Rectangle rect, string s_event, TRIGGER_TYPE type)
		: this(game, null, type)
	{
		m_class_name = "CursorTrigger";
		m_rect = new Rectangle(rect.X, rect.Y, rect.Width - rect.X, rect.Height - rect.Y);
		m_area_type = AREATRIGGER_TYPE.RECTANGLE;
		m_event = s_event;
		m_activate_own = true;
	}

	public CursorTrigger(Game game, SGSContentLoader CL, string content_path, Trigger trigger, TRIGGER_TYPE type)
		: this(game, trigger, type)
	{
		m_class_name = "CursorTrigger";
		m_texture = CL.LoadTexture(content_path);
		m_pixel_data = new Color[m_texture.Width * m_texture.Height];
		m_texture.GetData(m_pixel_data);
		m_area_type = AREATRIGGER_TYPE.ALPHA;
	}

	public CursorTrigger(Game game, SGSContentLoader CL, string content_path, string s_event, TRIGGER_TYPE type)
		: this(game, null, type)
	{
		m_class_name = "CursorTrigger";
		m_texture = CL.LoadTexture(content_path);
		m_pixel_data = new Color[m_texture.Width * m_texture.Height];
		m_texture.GetData(m_pixel_data);
		m_area_type = AREATRIGGER_TYPE.ALPHA;
		m_event = s_event;
		m_activate_own = true;
	}

	public override void Clear()
	{
		base.Clear();
		m_texture = null;
		m_pixel_data = null;
		if (m_trigger != null)
		{
			m_trigger.Clear();
			m_trigger = null;
		}
	}

	public void SetTriggerAnimation(Animation2D anim)
	{
		if (m_trigger != null)
		{
			ViewTrigger viewTrigger = (ViewTrigger)m_trigger;
			if (viewTrigger != null)
			{
				viewTrigger.m_animation = anim;
			}
		}
	}

	public override void Activate()
	{
		base.Activate();
		if (m_trigger != null)
		{
			m_trigger.Activate();
		}
	}

	protected virtual bool PixelCollision()
	{
		if (m_texture == null || m_pixel_data == null)
		{
			return false;
		}
		Vector2 vector = new Vector2(m_game.m_cursor.m_pos.X, m_game.m_cursor.m_pos.Y);
		vector.X *= 0.125f;
		vector.Y *= 0.125f;
		if (vector.X < 0f)
		{
			vector.X = 0f;
		}
		if (vector.Y < 0f)
		{
			vector.Y = 0f;
		}
		if (vector.X >= (float)m_texture.Width)
		{
			vector.X = m_texture.Width - 1;
		}
		if (vector.Y >= (float)m_texture.Height)
		{
			vector.Y = m_texture.Height - 1;
		}
		int num = (int)Math.Floor(vector.X) + (int)Math.Floor(vector.Y) * m_texture.Width;
		if (num < m_pixel_data.Length && m_pixel_data[num].A != 0)
		{
			return true;
		}
		return false;
	}

	public override void Update(TimeSpan elapsed)
	{
		if (!m_enabled || (m_game.m_tutorial_state != Tutorial.STATE.USE && m_game.m_tutorial_state != Tutorial.STATE.NONE && m_game.m_tutorial_state != Tutorial.STATE.WAIT_FOR_PICKUP))
		{
			return;
		}
		base.Update(elapsed);
		switch (m_area_type)
		{
		case AREATRIGGER_TYPE.RECTANGLE:
			switch (m_state)
			{
			case TRIGGER_STATE.IDLE:
				if (m_game.m_state != Game.GAME_STATE.ACTIVE_TRIGGER && m_game.m_cursor.m_pos.X >= (float)m_rect.Left && m_game.m_cursor.m_pos.X <= (float)m_rect.Right && m_game.m_cursor.m_pos.Y >= (float)m_rect.Top && m_game.m_cursor.m_pos.Y <= (float)m_rect.Bottom)
				{
					m_state = TRIGGER_STATE.OVER;
					m_game.m_over_trigger = this;
				}
				break;
			case TRIGGER_STATE.OVER:
				if (m_game.m_state != Game.GAME_STATE.ACTIVE_TRIGGER)
				{
					if (!(m_game.m_cursor.m_pos.X >= (float)m_rect.Left) || !(m_game.m_cursor.m_pos.X <= (float)m_rect.Right) || !(m_game.m_cursor.m_pos.Y >= (float)m_rect.Top) || !(m_game.m_cursor.m_pos.Y <= (float)m_rect.Bottom))
					{
						m_state = TRIGGER_STATE.IDLE;
					}
					else
					{
						m_game.m_over_trigger = this;
					}
				}
				break;
			case TRIGGER_STATE.ACTIVE:
				if (m_trigger != null)
				{
					m_trigger.Update(elapsed);
				}
				if (m_event != "")
				{
					m_game.HandleEvent(m_event);
					m_state = TRIGGER_STATE.IDLE;
				}
				break;
			}
			break;
		case AREATRIGGER_TYPE.ALPHA:
			switch (m_state)
			{
			case TRIGGER_STATE.IDLE:
				if (m_game.m_state != Game.GAME_STATE.ACTIVE_TRIGGER && PixelCollision())
				{
					m_state = TRIGGER_STATE.OVER;
					m_game.m_over_trigger = this;
				}
				break;
			case TRIGGER_STATE.OVER:
				if (m_game.m_state != Game.GAME_STATE.ACTIVE_TRIGGER)
				{
					if (!PixelCollision())
					{
						m_state = TRIGGER_STATE.IDLE;
					}
					else
					{
						m_game.m_over_trigger = this;
					}
				}
				break;
			case TRIGGER_STATE.ACTIVE:
				if (m_trigger != null)
				{
					m_trigger.Update(elapsed);
				}
				if (m_event != "")
				{
					m_game.HandleEvent(m_event);
					m_state = TRIGGER_STATE.IDLE;
				}
				break;
			}
			break;
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		base.Draw(SB);
		TRIGGER_STATE state = m_state;
		if (state == TRIGGER_STATE.ACTIVE && m_trigger != null)
		{
			m_trigger.Draw(SB);
		}
	}
}
