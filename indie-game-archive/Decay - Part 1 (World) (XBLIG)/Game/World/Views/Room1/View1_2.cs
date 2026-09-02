using Microsoft.Xna.Framework;

namespace Game.World.Views.Room1;

internal class View1_2 : View
{
	public View1_2(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "door")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_2_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_lock", trigger, Trigger.TRIGGER_TYPE.ZOOM_SMALL);
		m_triggers.Add(item);
		item = new CursorTrigger(m_game, new Rectangle(400, 115, 460, 132), "DoorHandle", Trigger.TRIGGER_TYPE.USE_SMALL);
		item.m_activate_own = true;
		m_triggers.Add(item);
	}

	public override void HandleEvent(string s_event)
	{
		base.HandleEvent(s_event);
	}
}
