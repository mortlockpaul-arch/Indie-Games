namespace Game.World.Views.Room2;

internal class View1_2 : View
{
	public View1_2(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "table")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "table_no_controller")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_remote", "Remote01", Trigger.TRIGGER_TYPE.USE_SMALL);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_2_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_suicide_note1", trigger, Trigger.TRIGGER_TYPE.USE_SMALL);
		m_triggers.Add(cursorTrigger);
		if (m_game.m_inventory.FindItem("Remote01"))
		{
			RemoveRemote01();
		}
	}

	private void RemoveRemote01()
	{
		ChangeScene(1);
		RemoveTrigger(m_triggers[0]);
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "PickupRemote01")
		{
			RemoveRemote01();
			m_game.m_cursor.onOut();
			FadeFromScene(0);
		}
		base.HandleEvent(s_event);
	}
}
