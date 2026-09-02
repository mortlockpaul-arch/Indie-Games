namespace Game.World.Views.Room2;

internal class View3_1 : View
{
	public View3_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View3_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "article_zoom")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View3"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, Game.VIEW_RECT, "View3_1.onZoom", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "View3_1.onZoom")
		{
			string text2 = "A traffic accident is believed to have occurred in the vicinity of Longlake,\n\r";
			text2 += "just outside Bright Valley. Three people, a man, woman, and a child,\n\r";
			text2 += "visiting the woman's father last week, are still missing.\n\r";
			text2 += "They were reported missing late on Friday by the woman's father, who\n\r";
			text2 += "became worried when they did not arrive. All contact has been lost to\n\r";
			text2 += "their mobile phones, yet some findings suggest they lost control of the\n\r";
			text2 += "car and drove into the water. Neither the car nor bodies have been found.\n\r\n\r";
			text2 += "- \"This is an incredible tragedy and our thoughts goes to the family's\n\r";
			text2 += "relatives and friends who now have to put up with this uncertainty about\n\r";
			text2 += "what might have happened,\" says Keith Johnsson, Information Manager at\n\r";
			text2 += "BVPD (Bright Valley Police Department)";
			m_game.m_hud.ShowText(text2, fade: true);
		}
		base.HandleEvent(s_event);
	}
}
