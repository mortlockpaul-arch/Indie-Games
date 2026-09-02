namespace Game.World.Views.Hallway1;

internal class View2_1 : View
{
	public View2_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View2_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "right_door_zoom_closed")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_right_door_zoom_closed", "View2_1.onDoor", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "View2_1.onDoor")
		{
			m_game.m_hud.ShowText("The door is locked ...", m_use_text_fade);
		}
		base.HandleEvent(s_event);
	}
}
