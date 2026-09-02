namespace Game.World.Views.Room2;

internal class View2_1 : View
{
	public View2_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View2_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "suicide_note_2_zoom")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, Game.VIEW_RECT, "View2_1.onZoom", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "View2_1.onZoom")
		{
			string text2 = "A few years ago my wife became very sick and passed away.\n\r";
			text2 += "Not long after that I lost my beloved daughter Emily,\n\r";
			text2 += "and my wonderful grandchild. Now I'm all alone in this world.\n\r";
			text2 += "I see no purpose in life.\n\r\n\r";
			text2 += "My body is in the bathroom.\n\r\n\r";
			text2 += "/Martin Wallace";
			m_game.m_hud.ShowText(text2, fade: true);
		}
		base.HandleEvent(s_event);
	}
}
