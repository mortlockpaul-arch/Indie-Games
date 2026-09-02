namespace Game.World.Views.Hallway1;

internal class View3_2 : View
{
	public View3_2(Game game, Area room)
		: base(game, room)
	{
		m_name = "View3_2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "poster_zoom")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View3"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, Game.VIEW_RECT, "View3_2.onPosters", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "View3_2.onPosters")
		{
			m_game.m_hud.ShowText("Just some posters on the wall ...", m_use_text_fade);
		}
		base.HandleEvent(s_event);
	}
}
