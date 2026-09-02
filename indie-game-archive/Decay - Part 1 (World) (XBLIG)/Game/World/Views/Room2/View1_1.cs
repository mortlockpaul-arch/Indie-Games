using Microsoft.Xna.Framework;

namespace Game.World.Views.Room2;

internal class View1_1 : View
{
	public View1_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "shelf")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "shelf_empty")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_frame", "Frame01", Trigger.TRIGGER_TYPE.USE_SMALL);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(337, 203, 405, 236), "View1_1.onBook", Trigger.TRIGGER_TYPE.ZOOM_SMALL);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(281, 65, 329, 135), "View1_1.onPicture", Trigger.TRIGGER_TYPE.ZOOM_SMALL);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		if (m_game.m_inventory.FindItem("Frame01"))
		{
			RemoveFrame01();
		}
	}

	private void RemoveFrame01()
	{
		ChangeScene(1);
		RemoveTrigger(m_triggers[0]);
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "PickupFrame01":
			RemoveFrame01();
			m_game.m_cursor.onOut();
			FadeFromScene(0);
			break;
		case "View1_1.onBook":
			m_game.m_hud.ShowText("The title of this book is \"Love & hate\". Seems dramatic ...", m_use_text_fade);
			break;
		case "View1_1.onPicture":
			m_game.m_hud.ShowText("It's a picture of an exterior environment.", m_use_text_fade);
			break;
		}
		base.HandleEvent(s_event);
	}
}
