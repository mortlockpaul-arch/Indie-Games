using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Game.World;

public class ViewTrigger : Trigger
{
	public enum VIEWTRIGGER_ANIM_TYPE
	{
		UNKNOWN,
		LEFT,
		RIGHT,
		UP,
		DOWN,
		LEFT_REVERSE,
		RIGHT_REVERSE,
		UP_REVERSE,
		DOWN_REVERSE,
		FADE_OUT,
		FADE_TO_BLACK
	}

	private enum VIEWTRIGGER_FADE_TO_BLACK_STATE
	{
		FADE_OUT,
		FADE_IN
	}

	private VIEWTRIGGER_FADE_TO_BLACK_STATE m_FTB_state;

	private float m_fade_alpha;

	public Animation2D m_animation;

	public bool m_reverse_animation;

	protected Game m_game;

	public View m_current_view;

	public View m_next_view;

	public VIEWTRIGGER_ANIM_TYPE m_view_type;

	public SoundEffect m_sound;

	public float m_sound_vol = 0.5f;

	public SoundEffectInstance m_stop_sound;

	public bool m_render_to_scene;

	protected bool m_do_render_to_scene;

	public string m_event = "";

	public ViewTrigger(Game game, View current_view, View next_view, VIEWTRIGGER_ANIM_TYPE type)
		: base(TRIGGER_TYPE.VIEW)
	{
		m_class_name = "ViewTrigger";
		m_game = game;
		m_current_view = current_view;
		m_next_view = next_view;
		m_view_type = type;
		if (m_view_type == VIEWTRIGGER_ANIM_TYPE.DOWN_REVERSE || m_view_type == VIEWTRIGGER_ANIM_TYPE.UP_REVERSE || m_view_type == VIEWTRIGGER_ANIM_TYPE.LEFT_REVERSE || m_view_type == VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE)
		{
			m_reverse_animation = true;
		}
	}

	public override void Clear()
	{
		m_sound = null;
		m_stop_sound = null;
		m_game = null;
		m_current_view = null;
		m_next_view = null;
		m_animation = null;
		base.Clear();
	}

	public override void Activate()
	{
		base.Activate();
		if (m_sound != null)
		{
			m_game.PlaySound(m_sound, m_sound_vol);
		}
		if (m_stop_sound != null)
		{
			m_stop_sound.Stop();
		}
		if (!m_do_render_to_scene && m_render_to_scene)
		{
			m_do_render_to_scene = true;
			return;
		}
		m_do_render_to_scene = false;
		switch (m_view_type)
		{
		case VIEWTRIGGER_ANIM_TYPE.UP:
		case VIEWTRIGGER_ANIM_TYPE.UP_REVERSE:
			m_animation = m_current_view.m_up_animation;
			break;
		case VIEWTRIGGER_ANIM_TYPE.DOWN:
		case VIEWTRIGGER_ANIM_TYPE.DOWN_REVERSE:
			m_animation = m_current_view.m_down_animation;
			break;
		case VIEWTRIGGER_ANIM_TYPE.LEFT:
		case VIEWTRIGGER_ANIM_TYPE.LEFT_REVERSE:
			m_animation = m_current_view.m_left_animation;
			break;
		case VIEWTRIGGER_ANIM_TYPE.RIGHT:
		case VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE:
			m_animation = m_current_view.m_right_animation;
			break;
		case VIEWTRIGGER_ANIM_TYPE.FADE_OUT:
			if (m_current_view.m_scenes[m_current_view.m_current_scene] != null)
			{
				m_current_view.m_scenes[m_current_view.m_current_scene].FadeOut();
			}
			break;
		case VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK:
			m_game.m_show_cursor = false;
			m_game.m_cursor.m_state = Cursor.CURSOR_STATE.IDLE;
			m_fade_alpha = 0f;
			m_FTB_state = VIEWTRIGGER_FADE_TO_BLACK_STATE.FADE_OUT;
			break;
		}
		if (m_animation != null)
		{
			m_animation.m_reverse = m_reverse_animation;
			m_animation.Play();
		}
		for (int i = 0; i < m_current_view.m_items.Count; i++)
		{
			if (m_current_view.m_items[i] != null)
			{
				switch (m_view_type)
				{
				case VIEWTRIGGER_ANIM_TYPE.UP:
				case VIEWTRIGGER_ANIM_TYPE.UP_REVERSE:
					m_current_view.m_items[i].PlayUpAnimation(m_reverse_animation);
					break;
				case VIEWTRIGGER_ANIM_TYPE.DOWN:
				case VIEWTRIGGER_ANIM_TYPE.DOWN_REVERSE:
					m_current_view.m_items[i].PlayDownAnimation(m_reverse_animation);
					break;
				case VIEWTRIGGER_ANIM_TYPE.LEFT:
				case VIEWTRIGGER_ANIM_TYPE.LEFT_REVERSE:
					m_current_view.m_items[i].PlayLeftAnimation(m_reverse_animation);
					break;
				case VIEWTRIGGER_ANIM_TYPE.RIGHT:
				case VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE:
					m_current_view.m_items[i].PlayRightAnimation(m_reverse_animation);
					break;
				case VIEWTRIGGER_ANIM_TYPE.FADE_OUT:
					m_current_view.m_items[i].FadeOut();
					break;
				}
			}
		}
		if (m_event != "")
		{
			m_game.HandleEvent(m_event);
		}
	}

	public override void Update(TimeSpan elapsed)
	{
		if (!m_enabled)
		{
			return;
		}
		base.Update(elapsed);
		if (m_do_render_to_scene)
		{
			return;
		}
		switch (m_state)
		{
		case TRIGGER_STATE.ACTIVE:
			if (m_view_type != VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK)
			{
				if (m_view_type == VIEWTRIGGER_ANIM_TYPE.FADE_OUT)
				{
					if (m_current_view.m_scenes[m_current_view.m_current_scene] != null)
					{
						m_current_view.m_scenes[m_current_view.m_current_scene].Update(elapsed);
					}
					if (m_next_view != null)
					{
						m_next_view.Update(elapsed);
					}
					if (m_current_view.m_scenes[m_current_view.m_current_scene].m_fade_state != Scene.FADE_STATE.IDLE)
					{
						break;
					}
				}
				else if (m_animation != null)
				{
					m_animation.Update(elapsed);
					if (m_current_view != null)
					{
						for (int i = 0; i < m_current_view.m_items.Count; i++)
						{
							if (m_current_view.m_items[i] != null)
							{
								m_current_view.m_items[i].Update(elapsed);
							}
						}
					}
					if (m_animation.m_state == Animation2D.ANIM_STATE.ANIM_STATE_PLAYING)
					{
						break;
					}
				}
			}
			else
			{
				if (m_current_view.m_scenes[m_current_view.m_current_scene] != null)
				{
					m_current_view.m_scenes[m_current_view.m_current_scene].Update(elapsed);
				}
				if (m_current_view != null)
				{
					m_current_view.UpdateLayerAnimations(elapsed);
				}
				if (m_next_view != null)
				{
					m_next_view.Update(elapsed);
				}
				if (m_FTB_state == VIEWTRIGGER_FADE_TO_BLACK_STATE.FADE_IN)
				{
					m_fade_alpha -= (float)elapsed.TotalMilliseconds * 0.001f * 400f;
					if (m_fade_alpha > 0f)
					{
						break;
					}
					m_game.m_show_cursor = true;
					m_game.onCursorOut();
				}
				else if (m_FTB_state == VIEWTRIGGER_FADE_TO_BLACK_STATE.FADE_OUT)
				{
					m_fade_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 400f;
					if (!(m_fade_alpha < 255f))
					{
						m_FTB_state = VIEWTRIGGER_FADE_TO_BLACK_STATE.FADE_IN;
					}
					break;
				}
			}
			if (m_game.m_world != null)
			{
				m_game.m_world.ChangeView(m_next_view);
			}
			m_state = TRIGGER_STATE.IDLE;
			m_game.m_d_up_pressed = true;
			m_game.m_d_down_pressed = true;
			m_game.m_d_left_pressed = true;
			m_game.m_d_right_pressed = true;
			break;
		case TRIGGER_STATE.IDLE:
			break;
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (m_do_render_to_scene)
		{
			try
			{
				m_current_view.RenderToScene(SB);
				Activate();
			}
			catch
			{
				m_do_render_to_scene = false;
			}
		}
		base.Draw(SB);
		if (SB == null)
		{
			return;
		}
		switch (m_state)
		{
		case TRIGGER_STATE.ACTIVE:
		{
			if (m_view_type != VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK)
			{
				if (m_next_view != null)
				{
					m_next_view.Draw(SB);
				}
				if (m_view_type == VIEWTRIGGER_ANIM_TYPE.FADE_OUT)
				{
					if (m_current_view != null)
					{
						m_current_view.Draw(SB);
					}
					break;
				}
				if (m_animation != null)
				{
					m_animation.Draw(SB);
				}
				if (m_current_view == null)
				{
					break;
				}
				for (int i = 0; i < m_current_view.m_items.Count; i++)
				{
					if (m_current_view.m_items[i] != null)
					{
						m_current_view.m_items[i].Draw(SB, Color.White);
					}
				}
				break;
			}
			if (m_FTB_state == VIEWTRIGGER_FADE_TO_BLACK_STATE.FADE_OUT)
			{
				if (m_current_view != null)
				{
					m_current_view.Draw(SB);
				}
				if (m_current_view != null)
				{
					for (int j = 0; j < m_current_view.m_items.Count; j++)
					{
						if (m_current_view.m_items[j] != null)
						{
							m_current_view.m_items[j].Draw(SB, Color.White);
						}
					}
				}
			}
			else if (m_FTB_state == VIEWTRIGGER_FADE_TO_BLACK_STATE.FADE_IN && m_next_view != null)
			{
				m_next_view.Draw(SB);
			}
			Color white = Color.White;
			if (m_fade_alpha <= 255f)
			{
				((Color)(ref white)).A = (byte)Math.Round(m_fade_alpha);
			}
			else
			{
				((Color)(ref white)).A = byte.MaxValue;
			}
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_game.m_fade_texture, Game.VIEW_RECT, white);
			SB.End();
			break;
		}
		case TRIGGER_STATE.IDLE:
			break;
		}
	}
}
