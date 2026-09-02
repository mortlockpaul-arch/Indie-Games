using Microsoft.Xna.Framework;

namespace Game.World.Views.Hallway1;

internal class View3 : View
{
	public View3(Game game, Area room)
		: base(game, room)
	{
		m_name = "View3";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "left_view")));
		m_right_animation = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to3/");
		m_left_animation = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View3to4/");
	}

	public override void Setup()
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		m_left_trigger = new ViewTrigger(m_game, this, m_room.GetView("View4"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT);
		m_right_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE);
		m_down_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View3_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_left_door", trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		trigger = new ViewTrigger(m_game, this, m_room.GetView("View3_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		item = new CursorTrigger(m_game, new Rectangle(64, 113, 233, 274), trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
	}

	public override void HandleEvent(string s_event)
	{
		base.HandleEvent(s_event);
	}
}
