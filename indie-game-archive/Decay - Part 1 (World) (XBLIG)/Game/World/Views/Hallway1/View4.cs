namespace Game.World.Views.Hallway1;

internal class View4 : View
{
	private Animation2D m_left_animation1;

	private Animation2D m_left_animation2;

	public View4(Game game, Area room)
		: base(game, room)
	{
		m_name = "View4";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "own_door")));
		m_left_animation1 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View2to4_closed/");
		m_left_animation2 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View2to4/");
		m_left_animation = m_left_animation1;
		m_right_animation = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View3to4/");
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
		base.Clear();
	}

	public override void Setup()
	{
		m_left_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT);
		m_right_trigger = new ViewTrigger(m_game, this, m_room.GetView("View3"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE);
		m_down_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_own_door", "Door_View4", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		if (m_game.m_inventory.FindItem("Note02"))
		{
			HandleNote02();
		}
	}

	private void HandleNote02()
	{
		m_left_animation = m_left_animation2;
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Door_View4":
			m_game.ChangeArea("Room2", "View1", door_sound: true);
			break;
		case "PickupNote02":
			HandleNote02();
			break;
		}
		base.HandleEvent(s_event);
	}
}
