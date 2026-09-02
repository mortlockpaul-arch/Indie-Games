using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game;

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

	public Vector2 m_pos;

	protected float m_speed;

	protected float m_default_speed;

	protected float m_slow_speed;

	protected float m_inc_speed_multiplier;

	protected float m_dec_speed_multiplier;

	protected Color m_color;

	protected bool m_slow_mode;

	public bool m_visible;

	public bool m_force_speed;

	public float m_custom_speed;

	public Cursor(Game game, Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		m_pos = Vector2.Zero;
		m_default_speed = 8f;
		m_slow_speed = 3f;
		m_inc_speed_multiplier = 1f;
		m_dec_speed_multiplier = 0.8f;
		m_color = Color.White;
		m_visible = true;
		m_custom_speed = 3f;
		base._002Ector();
		m_game = game;
		m_arrow_texture = ((Game)m_game).Content.Load<Texture2D>("Cursor/Animations/Arrow/0001");
		m_hand_texture = ((Game)m_game).Content.Load<Texture2D>("Cursor/Animations/Hand/0001");
		m_magnifier_texture = ((Game)m_game).Content.Load<Texture2D>("Cursor/Animations/Magnifier/0001");
		m_idle_texture = m_arrow_texture;
		m_arrow_anim = new TextureAnimation(m_game, ((Game)m_game).Content, "Cursor/Animations/Arrow/", 9u, reverse: false);
		m_arrow_anim.SetFPS(30.0);
		m_hand_anim = new TextureAnimation(m_game, ((Game)m_game).Content, "Cursor/Animations/Hand/", 9u, reverse: false);
		m_hand_anim.SetFPS(30.0);
		m_magnifier_anim = new TextureAnimation(m_game, ((Game)m_game).Content, "Cursor/Animations/Magnifier/", 9u, reverse: false);
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
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
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
			float speed = m_speed;
			GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadTriggers triggers = ((GamePadState)(ref state2)).Triggers;
			m_speed = speed + ((GamePadTriggers)(ref triggers)).Right * m_slow_speed * m_inc_speed_multiplier;
			float speed2 = m_speed;
			GamePadState state3 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadTriggers triggers2 = ((GamePadState)(ref state3)).Triggers;
			m_speed = speed2 - ((GamePadTriggers)(ref triggers2)).Left * m_slow_speed * m_dec_speed_multiplier;
		}
		else
		{
			m_speed = m_default_speed;
			float speed3 = m_speed;
			GamePadState state4 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadTriggers triggers3 = ((GamePadState)(ref state4)).Triggers;
			m_speed = speed3 + ((GamePadTriggers)(ref triggers3)).Right * m_default_speed * m_inc_speed_multiplier;
			float speed4 = m_speed;
			GamePadState state5 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadTriggers triggers4 = ((GamePadState)(ref state5)).Triggers;
			m_speed = speed4 - ((GamePadTriggers)(ref triggers4)).Left * m_default_speed * m_dec_speed_multiplier;
		}
		if (m_force_speed)
		{
			m_speed = m_custom_speed;
		}
		ref Vector2 pos = ref m_pos;
		float x = pos.X;
		GamePadState state6 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadThumbSticks thumbSticks = ((GamePadState)(ref state6)).ThumbSticks;
		pos.X = x + ((GamePadThumbSticks)(ref thumbSticks)).Left.X * m_speed;
		ref Vector2 pos2 = ref m_pos;
		float y = pos2.Y;
		GamePadState state7 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state7)).ThumbSticks;
		pos2.Y = y - ((GamePadThumbSticks)(ref thumbSticks2)).Left.Y * m_speed;
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)37))
		{
			if (((KeyboardState)(ref state)).IsKeyDown((Keys)160))
			{
				ref Vector2 pos3 = ref m_pos;
				pos3.X -= m_slow_speed;
			}
			else
			{
				ref Vector2 pos4 = ref m_pos;
				pos4.X -= m_default_speed;
			}
		}
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)39))
		{
			if (((KeyboardState)(ref state)).IsKeyDown((Keys)160))
			{
				ref Vector2 pos5 = ref m_pos;
				pos5.X += m_slow_speed;
			}
			else
			{
				ref Vector2 pos6 = ref m_pos;
				pos6.X += m_default_speed;
			}
		}
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)38))
		{
			if (((KeyboardState)(ref state)).IsKeyDown((Keys)160))
			{
				ref Vector2 pos7 = ref m_pos;
				pos7.Y -= m_slow_speed;
			}
			else
			{
				ref Vector2 pos8 = ref m_pos;
				pos8.Y -= m_default_speed;
			}
		}
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)40))
		{
			if (((KeyboardState)(ref state)).IsKeyDown((Keys)160))
			{
				ref Vector2 pos9 = ref m_pos;
				pos9.Y += m_slow_speed;
			}
			else
			{
				ref Vector2 pos10 = ref m_pos;
				pos10.Y += m_default_speed;
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

	public virtual void Draw(SpriteBatch SB)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		if (SB == null || !m_visible)
		{
			return;
		}
		int num = 60;
		switch (m_state)
		{
		case CURSOR_STATE.IDLE:
			if (m_idle_texture != null)
			{
				SB.Begin((SpriteBlendMode)1);
				SB.Draw(m_idle_texture, new Rectangle((int)m_pos.X - 6, (int)m_pos.Y - 3, num, num), (Rectangle?)new Rectangle(0, 0, 64, 64), m_color);
				SB.End();
			}
			break;
		case CURSOR_STATE.OVER:
			if (m_over_texture != null)
			{
				SB.Begin((SpriteBlendMode)1);
				SB.Draw(m_over_texture, new Rectangle((int)m_pos.X - 6, (int)m_pos.Y - 3, num, num), (Rectangle?)new Rectangle(0, 0, 64, 64), m_color);
				SB.End();
			}
			break;
		case CURSOR_STATE.OVER_ANIM:
			if (m_arrow_anim.m_state != Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_arrow_anim.Draw(SB, new Rectangle((int)m_pos.X - 6, (int)m_pos.Y - 3, num, num), new Rectangle(0, 0, 64, 64), m_color);
			}
			else if (m_over_anim != null)
			{
				m_over_anim.Draw(SB, new Rectangle((int)m_pos.X - 6, (int)m_pos.Y - 3, num, num), new Rectangle(0, 0, 64, 64), m_color);
			}
			break;
		case CURSOR_STATE.OUT_ANIM:
			if (m_out_anim.m_state != Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_out_anim.Draw(SB, new Rectangle((int)m_pos.X - 6, (int)m_pos.Y - 3, num, num), new Rectangle(0, 0, 64, 64), m_color);
			}
			else if (m_arrow_anim.m_state != Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_arrow_anim.Draw(SB, new Rectangle((int)m_pos.X - 6, (int)m_pos.Y - 3, num, num), new Rectangle(0, 0, 64, 64), m_color);
			}
			break;
		}
	}
}
