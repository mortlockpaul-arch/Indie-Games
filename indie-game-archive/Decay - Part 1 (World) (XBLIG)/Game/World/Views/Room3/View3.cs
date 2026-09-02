namespace Game.World.Views.Room3;

internal class View3 : View
{
	public View3(Game game, Area room)
		: base(game, room)
	{
		m_name = "View3";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "door_locked")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "door_open")));
	}

	public override void Setup()
	{
		m_right_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		m_down_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_door", "View3.onDoor", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		if (m_game.m_game_data.GetState("Room3.Completed") == "1")
		{
			HandleRoom3Completed();
		}
	}

	private void HandleRoom3Completed()
	{
		ChangeScene(1);
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Room3.Completed":
			m_game.m_game_data.SetState("Room3.Completed", "1");
			m_game.PlaySound(m_game.m_door_open, 0.5f, -1f);
			HandleRoom3Completed();
			break;
		case "View3.onDoor":
			if (m_current_scene == 0)
			{
				m_game.m_hud.ShowText("The door is stuck! It won't budge ...", m_use_text_fade);
			}
			else
			{
				m_game.ChangeArea("Hallway1", "View2", door_sound: true);
			}
			break;
		}
		base.HandleEvent(s_event);
	}
}
