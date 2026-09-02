using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Game.World;

public class CursorTrigger : Trigger
{
	public enum AREATRIGGER_TYPE
	{
		RECTANGLE,
		ALPHA
	}

	private AREATRIGGER_TYPE m_area_type;

	private Game m_game;

	private Rectangle m_rect;

	private Texture2D m_texture;

	private Trigger m_trigger;

	private Color[] m_pixel_data;

	private string m_event;

	public CursorTrigger(Game game, Trigger trigger, TRIGGER_TYPE type)
	{
		m_event = "";
		base._002Ector(type);
		m_class_name = "CursorTrigger";
		m_game = game;
		m_trigger = trigger;
	}

	public CursorTrigger(Game game, Rectangle rect, Trigger trigger, TRIGGER_TYPE type)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(game, trigger, type);
		m_class_name = "CursorTrigger";
		m_rect = new Rectangle(rect.X * 2, rect.Y * 2, rect.Width * 2 - rect.X * 2, rect.Height * 2 - rect.Y * 2);
		m_area_type = AREATRIGGER_TYPE.RECTANGLE;
	}

	public CursorTrigger(Game game, Rectangle rect, string s_event, TRIGGER_TYPE type)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(game, null, type);
		m_class_name = "CursorTrigger";
		m_rect = new Rectangle(rect.X * 2, rect.Y * 2, rect.Width * 2 - rect.X * 2, rect.Height * 2 - rect.Y * 2);
		m_area_type = AREATRIGGER_TYPE.RECTANGLE;
		m_event = s_event;
		m_activate_own = true;
	}

	public CursorTrigger(Game game, SGSContentLoader CL, string content_path, Trigger trigger, TRIGGER_TYPE type)
		: this(game, trigger, type)
	{
		m_class_name = "CursorTrigger";
		m_texture = CL.LoadTexture(content_path);
		m_pixel_data = (Color[])(object)new Color[m_texture.Width * m_texture.Height];
		m_texture.GetData<Color>(m_pixel_data);
		m_area_type = AREATRIGGER_TYPE.ALPHA;
	}

	public CursorTrigger(Game game, SGSContentLoader CL, string content_path, string s_event, TRIGGER_TYPE type)
		: this(game, null, type)
	{
		m_class_name = "CursorTrigger";
		m_texture = CL.LoadTexture(content_path);
		m_pixel_data = (Color[])(object)new Color[m_texture.Width * m_texture.Height];
		m_texture.GetData<Color>(m_pixel_data);
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

	private bool PixelCollision()
	{
		if (m_texture == null || m_pixel_data == null)
		{
			return false;
		}
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(m_game.m_cursor.m_pos.X, m_game.m_cursor.m_pos.Y);
		val.X *= 0.125f;
		val.Y *= 0.125f;
		if (val.X < 0f)
		{
			val.X = 0f;
		}
		if (val.Y < 0f)
		{
			val.Y = 0f;
		}
		if (val.X >= (float)m_texture.Width)
		{
			val.X = m_texture.Width - 1;
		}
		if (val.Y >= (float)m_texture.Height)
		{
			val.Y = m_texture.Height - 1;
		}
		if (((Color)(ref m_pixel_data[(int)Math.Floor(val.X) + (int)Math.Floor(val.Y) * m_texture.Width])).A != 0)
		{
			return true;
		}
		return false;
	}

	public override void Update(TimeSpan elapsed)
	{
		if (!m_enabled || (m_game.m_tutorial_state != Game.TUTORIAL_STATE.USE && m_game.m_tutorial_state != Game.TUTORIAL_STATE.NONE && m_game.m_tutorial_state != Game.TUTORIAL_STATE.WAIT_FOR_PICKUP))
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
				if (m_game.m_state != Game.GAME_STATE.ACTIVE_TRIGGER && m_game.m_cursor.m_pos.X >= (float)((Rectangle)(ref m_rect)).Left && m_game.m_cursor.m_pos.X <= (float)((Rectangle)(ref m_rect)).Right && m_game.m_cursor.m_pos.Y >= (float)((Rectangle)(ref m_rect)).Top && m_game.m_cursor.m_pos.Y <= (float)((Rectangle)(ref m_rect)).Bottom)
				{
					m_state = TRIGGER_STATE.OVER;
					m_game.onCursorOver(this);
				}
				break;
			case TRIGGER_STATE.OVER:
				if (m_game.m_state != Game.GAME_STATE.ACTIVE_TRIGGER && (!(m_game.m_cursor.m_pos.X >= (float)((Rectangle)(ref m_rect)).Left) || !(m_game.m_cursor.m_pos.X <= (float)((Rectangle)(ref m_rect)).Right) || !(m_game.m_cursor.m_pos.Y >= (float)((Rectangle)(ref m_rect)).Top) || !(m_game.m_cursor.m_pos.Y <= (float)((Rectangle)(ref m_rect)).Bottom)))
				{
					m_state = TRIGGER_STATE.IDLE;
					m_game.onCursorOut();
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
					m_game.onCursorOver(this);
				}
				break;
			case TRIGGER_STATE.OVER:
				if (m_game.m_state != Game.GAME_STATE.ACTIVE_TRIGGER && !PixelCollision())
				{
					m_state = TRIGGER_STATE.IDLE;
					m_game.onCursorOut();
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
