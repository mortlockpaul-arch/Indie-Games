using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Core.World;

public class World(Game game, string xml_path) : ScriptObject(game, xml_path)
{
	protected List<string> m_areas_to_load = new List<string>();

	protected List<Area> m_areas = new List<Area>();

	public Area m_current_area;

	public override void Clear()
	{
		m_current_area = null;
		if (m_areas != null)
		{
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
		if (m_areas_to_load != null)
		{
			m_areas_to_load.Clear();
			m_areas_to_load = null;
		}
		base.Clear();
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

	public virtual bool CreateHardcodedView(string view_path)
	{
		return false;
	}

	public Area GetCurrentArea()
	{
		return m_current_area;
	}

	public void ChangeArea(string name, string view, bool fade_in)
	{
		Area area = GetArea(name);
		if (area == null)
		{
			return;
		}
		if (m_current_area != null)
		{
			m_current_area.Clear();
			m_current_area = null;
		}
		m_current_area = area;
		m_current_area.Load(m_game);
		float num = (float)((double)GC.GetTotalMemory(forceFullCollection: false) * 1E-06);
		num = (float)Math.Round(num, 1);
		Console.WriteLine("Texture memory for current area: " + num + "MB");
		View view2 = m_current_area.GetView(view);
		if (view2 == null)
		{
			Console.WriteLine("ChangeArea: view == null, shutting down!");
			m_game.onExitGame();
			m_game.Exit();
			return;
		}
		m_game.m_game_data.m_area = m_current_area.m_name;
		m_game.m_game_data.m_view = view;
		ChangeView(view2);
		if (fade_in)
		{
			m_game.FadeInArea();
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
		}
		m_current_area.m_current_view.Reset();
		m_game.ClearTrigger();
		m_game.m_b_pressed = true;
		m_game.m_d_down_pressed = true;
	}

	public override void HandleEvent(string s_event)
	{
		base.HandleEvent(s_event);
		if (m_current_area != null)
		{
			m_current_area.HandleEvent(s_event);
		}
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
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
