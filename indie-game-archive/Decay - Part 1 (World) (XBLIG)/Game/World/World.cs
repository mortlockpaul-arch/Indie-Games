using System;
using System.Collections.Generic;
using Game.World.Areas;
using Microsoft.Xna.Framework.Graphics;

namespace Game.World;

public class World
{
	public string m_name = "";

	protected List<Area> m_areas = new List<Area>();

	protected Game m_game;

	public Area m_current_area;

	public World(Game game)
	{
		m_game = game;
		m_areas.Add(new Room1());
		m_areas.Add(new Room2());
		m_areas.Add(new Hallway1());
		m_areas.Add(new Room3());
	}

	public virtual void Clear()
	{
		m_game = null;
		m_current_area = null;
		if (m_areas == null)
		{
			return;
		}
		for (int i = 0; i < m_areas.Count; i++)
		{
			if (m_areas[i] != null)
			{
				m_areas[i].Clear();
				m_areas[i] = null;
			}
		}
		m_areas.Clear();
		m_areas = null;
	}

	public Area GetArea(string name)
	{
		if (m_areas != null)
		{
			for (int i = 0; i < m_areas.Count; i++)
			{
				if (m_areas[i] != null && m_areas[i].m_name == name)
				{
					return m_areas[i];
				}
			}
		}
		return null;
	}

	public View GetCurrentView()
	{
		if (m_current_area != null && m_current_area.m_current_view != null)
		{
			return m_current_area.m_current_view;
		}
		return null;
	}

	public void ChangeArea(string name, string view, bool fade_in)
	{
		Area area = GetArea(name);
		if (area != null)
		{
			if (m_current_area != null)
			{
				m_current_area.Clear();
				m_current_area = null;
			}
			m_current_area = area;
			m_current_area.Load(m_game);
			ChangeView(m_current_area.GetView(view));
			m_game.m_game_data.m_area = m_current_area.m_name;
			m_game.m_game_data.m_view = view;
			if (fade_in)
			{
				m_game.FadeInArea();
			}
		}
	}

	public void ChangeView(View view)
	{
		if (view != null)
		{
			m_game.m_game_data.m_view = view.m_name;
		}
		if (m_current_area != null)
		{
			m_current_area.m_current_view = view;
			if (m_game.m_hud != null && m_game.m_hud.m_navigator != null)
			{
				m_game.m_hud.m_navigator.Setup(m_current_area.m_current_view.m_left_trigger != null, m_current_area.m_current_view.m_right_trigger != null, m_current_area.m_current_view.m_up_trigger != null, m_current_area.m_current_view.m_down_trigger != null);
			}
		}
		m_game.ClearTrigger();
		m_game.m_b_pressed = true;
		m_game.m_d_down_pressed = true;
		m_current_area.m_current_view.Reset();
	}

	public virtual void HandleEvent(string s_event)
	{
		if (m_current_area != null)
		{
			m_current_area.HandleEvent(s_event);
		}
	}

	public virtual void HandleUseEvent(string s_event)
	{
		if (m_current_area != null)
		{
			m_current_area.HandleUseEvent(s_event);
		}
	}

	public virtual void Update(TimeSpan elapsed)
	{
		if (m_current_area != null)
		{
			m_current_area.Update(elapsed);
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
		if (m_current_area != null)
		{
			m_current_area.Draw(SB);
		}
	}

	public virtual void UpdateEffect(TimeSpan elapsed)
	{
		if (m_current_area != null)
		{
			m_current_area.UpdateEffect(elapsed);
		}
	}

	public virtual void DrawEffect(SpriteBatch SB)
	{
		if (m_current_area != null)
		{
			m_current_area.DrawEffect(SB);
		}
	}
}
