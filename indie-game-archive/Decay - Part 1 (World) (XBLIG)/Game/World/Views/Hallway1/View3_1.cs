using Microsoft.Xna.Framework;

namespace Game.World.Views.Hallway1;

internal class View3_1 : View
{
	public View3_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View3_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "Left room/left_room_upsidedown_with_note")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "Left room/left_room_upsidedown_without_note")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "Left room/left_room_with_battery")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "Left room/left_room_without_battery")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View3"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View3_1_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger item = new CursorTrigger(m_game, new Rectangle(290, 285, 342, 315), trigger, Trigger.TRIGGER_TYPE.ZOOM_SMALL);
		m_triggers.Add(item);
		trigger = new ViewTrigger(m_game, this, m_room.GetView("View3_1_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		item = new CursorTrigger(m_game, new Rectangle(302, 16, 341, 69), trigger, Trigger.TRIGGER_TYPE.ZOOM_SMALL);
		m_triggers.Add(item);
		trigger = new ViewTrigger(m_game, this, m_room.GetView("View3_1_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		item = new CursorTrigger(m_game, new Rectangle(302, 254, 339, 303), trigger, Trigger.TRIGGER_TYPE.ZOOM_SMALL);
		item.m_enabled = false;
		m_triggers.Add(item);
		trigger = new ViewTrigger(m_game, this, m_room.GetView("View3_1_3"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		item = new CursorTrigger(m_game, new Rectangle(366, 137, 405, 179), trigger, Trigger.TRIGGER_TYPE.ZOOM_SMALL);
		m_triggers.Add(item);
		trigger = new ViewTrigger(m_game, this, m_room.GetView("View3_1_3"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		item = new CursorTrigger(m_game, new Rectangle(237, 157, 277, 199), trigger, Trigger.TRIGGER_TYPE.ZOOM_SMALL);
		item.m_enabled = false;
		m_triggers.Add(item);
		if (m_game.m_inventory.FindItem("Note02"))
		{
			HandleNote02();
		}
		if (m_game.m_game_data.GetState("Hallway1.GotBattery01") == "1")
		{
			HandleBattery01();
		}
	}

	private void HandleNote02()
	{
		ChangeScene(1);
		m_triggers[0].m_enabled = false;
	}

	private void HandleRoom3Completed()
	{
		if (!(m_game.m_game_data.GetState("Hallway1.GotBattery01") == "1"))
		{
			ChangeScene(2);
			m_triggers[0].m_enabled = false;
			m_triggers[1].m_enabled = false;
			m_triggers[2].m_enabled = true;
			m_triggers[3].m_enabled = false;
			m_triggers[4].m_enabled = true;
		}
	}

	private void HandleBattery01()
	{
		ChangeScene(3);
		m_triggers[0].m_enabled = false;
		m_triggers[1].m_enabled = false;
		m_triggers[2].m_enabled = false;
		m_triggers[3].m_enabled = false;
		m_triggers[4].m_enabled = true;
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "PickupNote02":
			HandleNote02();
			break;
		case "Room3.Completed":
			HandleRoom3Completed();
			break;
		case "PickupBattery01":
			HandleBattery01();
			break;
		}
		base.HandleEvent(s_event);
	}
}
