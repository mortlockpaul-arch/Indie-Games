using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Game.World;

public class Area
{
	public string m_name = "";

	protected List<View> m_views;

	protected Game m_game;

	public View m_current_view;

	public SGSContentLoader m_CL;

	public string m_content_path = "";

	protected Animation2D m_anim;

	protected bool m_play_anim;

	protected string m_anim_event = "";

	public virtual void Load(Game game)
	{
		Clear();
		m_game = game;
		m_views = new List<View>();
		m_CL = new SGSContentLoader((IServiceProvider)((Game)game).Services);
	}

	public virtual void Clear()
	{
		m_game = null;
		m_anim = null;
		m_current_view = null;
		if (m_views != null)
		{
			for (int i = 0; i < m_views.Count; i++)
			{
				if (m_views[i] != null)
				{
					m_views[i].Clear();
					m_views[i] = null;
				}
			}
			m_views.Clear();
			m_views = null;
		}
		if (m_CL != null)
		{
			m_CL.Clear();
			m_CL = null;
		}
	}

	public virtual void Init()
	{
		m_game.m_show_cursor = true;
	}

	public virtual void HandleEvent(string s_event)
	{
		for (int i = 0; i < m_views.Count; i++)
		{
			if (m_views[i] != null)
			{
				m_views[i].HandleEvent(s_event);
			}
		}
	}

	public virtual void HandleUseEvent(string s_event)
	{
		if (m_current_view != null && !m_current_view.HandleUseEvent(s_event))
		{
			m_game.m_hud.ShowText("Can not use this here ...", m_current_view.m_use_text_fade);
			m_current_view.ResetCursorTriggers();
		}
	}

	public void AddView(View view)
	{
		if (view != null)
		{
			m_views.Add(view);
		}
	}

	public View GetView(string name)
	{
		if (m_views != null)
		{
			for (int i = 0; i < m_views.Count; i++)
			{
				if (m_views[i] != null && m_views[i].m_name == name)
				{
					return m_views[i];
				}
			}
		}
		return null;
	}

	protected void SetupViews()
	{
		if (m_views == null)
		{
			return;
		}
		for (int i = 0; i < m_views.Count; i++)
		{
			if (m_views[i] != null)
			{
				m_views[i].Setup();
			}
		}
	}

	public virtual void Update(TimeSpan elapsed)
	{
		if (m_play_anim)
		{
			if (m_anim != null)
			{
				m_anim.Update(elapsed);
				if (m_anim.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
				{
					m_play_anim = false;
					m_game.m_show_cursor = true;
					m_game.m_input_enabled = true;
					m_game.m_inventory_enabled = true;
					m_game.m_hud.FadeIn();
					m_game.HandleEvent(m_anim_event);
				}
			}
		}
		else if (m_current_view != null)
		{
			m_current_view.Update(elapsed);
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
		if (m_play_anim)
		{
			if (m_anim != null)
			{
				m_anim.Draw(SB);
			}
		}
		else if (m_current_view != null)
		{
			m_current_view.Draw(SB);
		}
	}

	public virtual void UpdateEffect(TimeSpan elapsed)
	{
		if (m_current_view != null)
		{
			m_current_view.UpdateEffect(elapsed);
		}
	}

	public virtual void DrawEffect(SpriteBatch SB)
	{
		if (m_current_view != null)
		{
			m_current_view.DrawEffect(SB);
		}
	}

	public virtual void PlayAnimation(Animation2D anim, string s_event)
	{
		if (anim != null)
		{
			m_anim = anim;
			m_anim.Play();
			m_play_anim = true;
			m_anim_event = s_event;
			m_game.m_show_cursor = false;
			m_game.m_input_enabled = false;
			m_game.m_inventory_enabled = false;
			m_game.m_hud.FadeOut();
		}
	}
}
