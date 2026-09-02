namespace Game.World.Views.Room2;

internal class View1_2_1 : View
{
	public View1_2_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_2_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "suicide_note1_zoom")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, Game.VIEW_RECT, "View1_2_1.onNote", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "View1_2_1.onNote")
		{
			m_game.m_hud.ShowText("Did I write this note? I ... I can't remember ...", m_use_text_fade);
		}
		base.HandleEvent(s_event);
	}
}
