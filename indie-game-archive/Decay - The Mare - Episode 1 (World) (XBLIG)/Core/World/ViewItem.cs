using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Core.World;

public class ViewItem
{
	public enum VIEWITEM_STATE
	{
		REMOVED,
		SCENE,
		FADE_OUT,
		FADE_OUT_REMOVE,
		ANIM_LEFT,
		ANIM_RIGHT,
		ANIM_UP,
		ANIM_DOWN
	}

	public string m_name = "";

	protected Game m_game;

	protected Texture2D m_scene;

	protected Animation2D m_fade_animation;

	protected Animation2D m_up_animation;

	protected Animation2D m_down_animation;

	protected Animation2D m_left_animation;

	protected Animation2D m_right_animation;

	protected Rectangle m_source_rect = Rectangle.Empty;

	protected Rectangle m_dest_rect = Rectangle.Empty;

	protected Rectangle m_anim_source_rect = Rectangle.Empty;

	protected Animation2D m_animation;

	public VIEWITEM_STATE m_state = VIEWITEM_STATE.SCENE;

	public bool m_update_animation = true;

	public List<Animation2D> m_anims_to_update = new List<Animation2D>();

	public ViewItem(string name, Game game, Rectangle rect)
	{
		m_name = name;
		m_game = game;
		m_source_rect = rect;
		m_fade_animation = new AlphaAnimation(m_game, 15u, reverse: false, m_scene);
	}

	public ViewItem(string name, Game game, SGSContentLoader CL, string scene, Rectangle rect)
	{
		m_name = name;
		m_game = game;
		m_scene = CL.LoadTexture(scene);
		double num = (double)m_scene.Width / 640.0;
		double num2 = (double)m_scene.Height / 360.0;
		m_source_rect = new Rectangle((int)Math.Round((double)rect.Left * num), (int)Math.Round((double)rect.Top * num2), (int)Math.Round((double)rect.Width * num), (int)Math.Round((double)rect.Height * num2));
		m_anim_source_rect = rect;
		m_anim_source_rect.X /= 2;
		m_anim_source_rect.Y /= 2;
		m_anim_source_rect.Width /= 2;
		m_anim_source_rect.Height /= 2;
		m_dest_rect = rect;
		m_dest_rect.X *= 2;
		m_dest_rect.Y *= 2;
		m_dest_rect.Width *= 2;
		m_dest_rect.Height *= 2;
		m_fade_animation = new AlphaAnimation(m_game, 15u, reverse: false, m_scene);
	}

	public virtual void Clear()
	{
		m_game = null;
		m_scene = null;
		m_animation = null;
		for (int i = 0; i < m_anims_to_update.Count; i++)
		{
			m_anims_to_update[i] = null;
		}
		m_anims_to_update.Clear();
		m_anims_to_update = null;
		if (m_up_animation != null)
		{
			m_up_animation.Clear();
			m_up_animation = null;
		}
		if (m_down_animation != null)
		{
			m_down_animation.Clear();
			m_down_animation = null;
		}
		if (m_left_animation != null)
		{
			m_left_animation.Clear();
			m_left_animation = null;
		}
		if (m_right_animation != null)
		{
			m_right_animation.Clear();
			m_right_animation = null;
		}
	}

	public void LoadLeftAnimation(TextureAnimation anim)
	{
		m_left_animation = anim;
	}

	public void LoadRightAnimation(TextureAnimation anim)
	{
		m_right_animation = anim;
	}

	public void LoadUpAnimation(TextureAnimation anim)
	{
		m_up_animation = anim;
	}

	public void LoadDownAnimation(TextureAnimation anim)
	{
		m_down_animation = anim;
	}

	public void Remove()
	{
		m_state = VIEWITEM_STATE.REMOVED;
	}

	public void FadeOut()
	{
		if (m_state != VIEWITEM_STATE.REMOVED)
		{
			m_fade_animation.Play();
			m_state = VIEWITEM_STATE.FADE_OUT;
		}
	}

	public void FadeOutRemove()
	{
		if (m_state != VIEWITEM_STATE.REMOVED)
		{
			m_fade_animation.Play();
			m_state = VIEWITEM_STATE.FADE_OUT_REMOVE;
		}
	}

	public void PlayLeftAnimation(bool reverse)
	{
		if (m_left_animation != null)
		{
			m_left_animation.m_reverse = reverse;
			m_left_animation.Play();
			m_animation = m_left_animation;
			if (m_state != VIEWITEM_STATE.REMOVED)
			{
				m_state = VIEWITEM_STATE.ANIM_LEFT;
			}
		}
	}

	public void PlayRightAnimation(bool reverse)
	{
		if (m_right_animation != null)
		{
			m_right_animation.m_reverse = reverse;
			m_right_animation.Play();
			m_animation = m_right_animation;
			if (m_state != VIEWITEM_STATE.REMOVED)
			{
				m_state = VIEWITEM_STATE.ANIM_RIGHT;
			}
		}
	}

	public void PlayUpAnimation(bool reverse)
	{
		if (m_up_animation != null)
		{
			m_up_animation.m_reverse = reverse;
			m_up_animation.Play();
			if (m_state != VIEWITEM_STATE.REMOVED)
			{
				m_state = VIEWITEM_STATE.ANIM_UP;
			}
		}
	}

	public void PlayDownAnimation(bool reverse)
	{
		if (m_down_animation != null)
		{
			m_down_animation.m_reverse = reverse;
			m_down_animation.Play();
			if (m_state != VIEWITEM_STATE.REMOVED)
			{
				m_state = VIEWITEM_STATE.ANIM_DOWN;
			}
		}
	}

	public void Update(TimeSpan elapsed)
	{
		if (m_animation != null && m_update_animation)
		{
			m_animation.Update(elapsed);
		}
		if (m_anims_to_update != null)
		{
			for (int i = 0; i < m_anims_to_update.Count; i++)
			{
				if (m_anims_to_update[i] != null)
				{
					m_anims_to_update[i].Update(elapsed);
				}
			}
		}
		switch (m_state)
		{
		case VIEWITEM_STATE.FADE_OUT:
			if (m_fade_animation != null)
			{
				m_fade_animation.Update(elapsed);
				if (m_fade_animation.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
				{
					m_state = VIEWITEM_STATE.SCENE;
				}
			}
			break;
		case VIEWITEM_STATE.FADE_OUT_REMOVE:
			if (m_fade_animation != null)
			{
				m_fade_animation.Update(elapsed);
				if (m_fade_animation.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
				{
					m_state = VIEWITEM_STATE.REMOVED;
				}
			}
			break;
		case VIEWITEM_STATE.ANIM_LEFT:
			if (m_left_animation != null && m_left_animation.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_animation = null;
				m_state = VIEWITEM_STATE.SCENE;
			}
			break;
		case VIEWITEM_STATE.ANIM_RIGHT:
			if (m_right_animation != null && m_right_animation.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_animation = null;
				m_state = VIEWITEM_STATE.SCENE;
			}
			break;
		case VIEWITEM_STATE.ANIM_UP:
			if (m_up_animation != null)
			{
				if (m_update_animation)
				{
					m_up_animation.Update(elapsed);
				}
				if (m_up_animation.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
				{
					m_state = VIEWITEM_STATE.SCENE;
				}
			}
			break;
		case VIEWITEM_STATE.ANIM_DOWN:
			if (m_down_animation != null)
			{
				if (m_update_animation)
				{
					m_down_animation.Update(elapsed);
				}
				if (m_down_animation.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
				{
					m_state = VIEWITEM_STATE.SCENE;
				}
			}
			break;
		case VIEWITEM_STATE.REMOVED:
		case VIEWITEM_STATE.SCENE:
			break;
		}
	}

	public void Draw(SpriteBatch SB, Color color)
	{
		switch (m_state)
		{
		case VIEWITEM_STATE.SCENE:
			if (m_scene != null)
			{
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_scene, m_dest_rect, m_source_rect, color);
				SB.End();
			}
			break;
		case VIEWITEM_STATE.FADE_OUT_REMOVE:
			if (m_fade_animation != null)
			{
				m_fade_animation.Draw(SB, m_dest_rect, m_source_rect, color);
			}
			break;
		case VIEWITEM_STATE.ANIM_LEFT:
			if (m_animation != null)
			{
				m_animation.Draw(SB, m_dest_rect, m_anim_source_rect, color);
			}
			break;
		case VIEWITEM_STATE.ANIM_RIGHT:
			if (m_animation != null)
			{
				m_animation.Draw(SB, m_dest_rect, m_anim_source_rect, color);
			}
			break;
		case VIEWITEM_STATE.ANIM_UP:
			if (m_up_animation != null)
			{
				m_up_animation.Draw(SB, m_dest_rect, m_anim_source_rect, color);
			}
			break;
		case VIEWITEM_STATE.ANIM_DOWN:
			if (m_down_animation != null)
			{
				m_down_animation.Draw(SB, m_dest_rect, m_anim_source_rect, color);
			}
			break;
		case VIEWITEM_STATE.REMOVED:
		case VIEWITEM_STATE.FADE_OUT:
			break;
		}
	}
}
