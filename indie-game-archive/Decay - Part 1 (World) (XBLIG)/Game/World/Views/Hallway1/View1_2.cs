using System;
using Microsoft.Xna.Framework;

namespace Game.World.Views.Hallway1;

internal class View1_2 : View
{
	private float m_light_timer;

	private int m_light_timeout_min = 100;

	private int m_light_timeout_max = 1500;

	public View1_2(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "exit_zoom")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "exit_zoom_nolamp")));
		m_light_timer = m_light_timeout_min;
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, new Rectangle(264, 77, 377, 304), "View1_2.onExit", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "View1_2.onExit")
		{
			m_game.m_hud.ShowText("The door has been barricaded. I wonder why ...", m_use_text_fade);
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
		m_light_timer -= (float)elapsed.TotalMilliseconds;
		if (m_light_timer <= 0f)
		{
			m_light_timer = m_game.GetRandom(m_light_timeout_min, m_light_timeout_max);
			if (m_current_scene == 0)
			{
				ChangeScene(1);
			}
			else
			{
				ChangeScene(0);
			}
		}
	}
}
