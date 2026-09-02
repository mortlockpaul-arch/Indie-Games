namespace Game.World.Views.Hallway1;

internal class View2 : View
{
	private Animation2D m_left_animation1;

	private Animation2D m_left_animation2;

	private Animation2D m_right_animation1;

	private Animation2D m_right_animation2;

	public View2(Game game, Area room)
		: base(game, room)
	{
		m_name = "View2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "right_view_closed")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "right_view")));
		m_left_animation1 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2_closed/");
		m_left_animation2 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2/");
		m_left_animation = m_left_animation1;
		m_right_animation1 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View2to4_closed/");
		m_right_animation2 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View2to4/");
		m_right_animation = m_right_animation1;
	}

	public override void Clear()
	{
		if (m_left_animation1 != null)
		{
			m_left_animation1.Clear();
			m_left_animation1 = null;
		}
		if (m_left_animation2 != null)
		{
			m_left_animation2.Clear();
			m_left_animation2 = null;
		}
		if (m_right_animation1 != null)
		{
			m_right_animation1.Clear();
			m_right_animation1 = null;
		}
		if (m_right_animation2 != null)
		{
			m_right_animation2.Clear();
			m_right_animation2 = null;
		}
		base.Clear();
	}

	public override void Setup()
	{
		m_left_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT);
		m_right_trigger = new ViewTrigger(m_game, this, m_room.GetView("View4"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE);
		m_down_trigger = new ViewTrigger(m_game, this, m_room.GetView("View3"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_right_door", "View2.Door", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View2_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_right_door", trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(cursorTrigger);
		if (m_game.m_inventory.FindItem("Note02"))
		{
			HandleNote02();
		}
	}

	private void HandleNote02()
	{
		ChangeScene(1);
		m_triggers[0].m_enabled = true;
		m_triggers[1].m_enabled = false;
		m_left_animation = m_left_animation2;
		m_right_animation = m_right_animation2;
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "View2.Door":
			m_game.FadeOutMusic();
			m_game.m_play_door_sound = false;
			m_game.ChangeArea("Room3", "View1", door_sound: true);
			break;
		case "PickupNote02":
			HandleNote02();
			break;
		}
		base.HandleEvent(s_event);
	}
}
