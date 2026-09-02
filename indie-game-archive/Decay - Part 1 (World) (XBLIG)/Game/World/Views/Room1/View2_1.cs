namespace Game.World.Views.Room1;

public class View2_1 : View
{
	public View2_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View2_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "handfat_med_nyckel")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "handfat_utan_nyckel")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_nyckel", "Key01", Trigger.TRIGGER_TYPE.USE_SMALL);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		if (m_game.m_inventory.FindItem("Key01") || m_game.m_game_data.GetState("Room1.Door") == "Unlocked")
		{
			HandleKey01();
		}
	}

	private void HandleKey01()
	{
		ChangeScene(1);
		RemoveTrigger(m_triggers[0]);
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "PickupKey01")
		{
			HandleKey01();
			m_game.m_cursor.onOut();
			FadeFromScene(0);
		}
		base.HandleEvent(s_event);
	}
}
