using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Core.World;

public class Area : ScriptObject
{
	protected List<View> m_views;

	public View m_current_view;

	public SGSContentLoader m_CL;

	public string m_content_path = "";

	protected Animation2D m_anim;

	protected bool m_play_anim;

	protected string m_anim_event = "";

	protected string m_view_path = "";

	public Area(Game game, string xml_path, string name)
		: base(game, xml_path)
	{
		m_name = name;
	}

	public virtual void Load(Game game)
	{
		Clear();
		m_game = game;
		m_views = new List<View>();
		m_CL = new SGSContentLoader(game.Services);
		LoadXML();
		if (m_xml_doc == null)
		{
			return;
		}
		XNode xNode = m_xml_doc.Root.FirstNode;
		XElement xElement = null;
		while (xNode != null)
		{
			if ((object)xNode.GetType() == typeof(XElement))
			{
				xElement = (XElement)xNode;
				if (!parseElement(xElement))
				{
					switch (xElement.Name.ToString())
					{
					case "ViewPath":
						m_view_path = xElement.Value;
						break;
					case "View":
						if (!m_game.m_world.CreateHardcodedView(m_view_path + xElement.Value))
						{
							new View(m_game, this, m_view_path + xElement.Value);
						}
						break;
					}
				}
			}
			xNode = xNode.NextNode;
		}
		xElement = null;
		xNode = null;
		m_game.HandleEvent(m_name + ".onLoaded");
	}

	public override void Clear()
	{
		base.Clear();
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
		if (!m_game.m_input_blocked)
		{
			m_game.m_show_cursor = true;
		}
		m_game.HandleEvent(m_name + ".Init");
	}

	protected override SGSContentLoader getContentLoader()
	{
		return m_CL;
	}

	public override void HandleEvent(string s_event)
	{
		base.HandleEvent(s_event);
		for (int i = 0; i < m_views.Count; i++)
		{
			if (m_views[i] != null)
			{
				m_views[i].HandleEvent(s_event);
			}
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

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
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
