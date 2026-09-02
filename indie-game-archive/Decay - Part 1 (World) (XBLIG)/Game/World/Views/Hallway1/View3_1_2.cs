using Microsoft.Xna.Framework;

namespace Game.World.Views.Hallway1;

internal class View3_1_2 : View
{
	public View3_1_2(Game game, Area room)
		: base(game, room)
	{
		m_name = "View3_1_2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "Left room/battery_zoom_upsidedown")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "Left room/battery_zoom")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "Left room/battery_zoom2")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View3_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, new Rectangle(290, 113, 343, 181), "View3_1_2.onBattery", Trigger.TRIGGER_TYPE.ZOOM_SMALL);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(288, 151, 352, 234), "Battery01", Trigger.TRIGGER_TYPE.USE_SMALL);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		if (m_game.m_game_data.GetState("Hallway1.GotBattery01") == "1")
		{
			HandleBattery01();
		}
	}

	private void HandleRoom3Completed()
	{
		if (!(m_game.m_game_data.GetState("Hallway1.GotBattery01") == "1"))
		{
			ChangeScene(1);
			m_triggers[0].m_enabled = false;
			m_triggers[1].m_enabled = true;
		}
	}

	private void HandleBattery01()
	{
		ChangeScene(2);
		m_triggers[0].m_enabled = false;
		m_triggers[1].m_enabled = false;
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Battery01":
			m_game.m_inventory.AskPickup(s_event);
			break;
		case "PickupBattery01":
			m_game.m_game_data.SetState("Hallway1.GotBattery01", "1");
			m_game.m_inventory.AddItem("Battery01");
			HandleBattery01();
			m_game.m_cursor.onOut();
			break;
		case "View3_1_2.onBattery":
			m_game.m_hud.ShowText("Looks like a battery. I can't reach it from here ...", m_use_text_fade);
			break;
		case "Room3.Completed":
			HandleRoom3Completed();
			break;
		}
		base.HandleEvent(s_event);
	}
}
