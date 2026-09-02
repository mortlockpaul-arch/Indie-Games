namespace Game.World.Views.Hallway1;

internal class View3_1_3 : View
{
	public View3_1_3(Game game, Area room)
		: base(game, room)
	{
		m_name = "View3_1_3";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "Left room/zoom_still_at_large_flipped")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "Left room/zoom_still_at_large")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View3_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, Game.VIEW_RECT, "View3_1_3.onNote", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	private void HandleRoom3Completed()
	{
		m_triggers[0].m_enabled = true;
		ChangeScene(1);
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "View3_1_3.onNote":
			if (m_current_scene == 1)
			{
				string text = "\"The pale man\" has still not been arrested. The man who is\n\r";
				text += "believed to be responsible for dozens of killings in the state of\n\r";
				text += "Middlepad is described as a man in his fifties who is very thin,\n\r";
				text += "tall and bald. The police say the killer may have been wearing a\n\r";
				text += "dark hoodie, dark pants and army boots during some of the\n\r";
				text += "killings.";
				m_game.m_hud.ShowText(text, m_use_text_fade);
			}
			else
			{
				string text2 = "It's upside down ...";
				m_game.m_hud.ShowText(text2, m_use_text_fade);
			}
			break;
		case "Room3.Completed":
			HandleRoom3Completed();
			break;
		}
		base.HandleEvent(s_event);
	}
}
