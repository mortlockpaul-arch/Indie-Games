using Microsoft.Xna.Framework;

namespace Game.World.Views.Hallway1;

internal class View1_1_2_2 : View
{
	public View1_1_2_2(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_1_2_2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "doll_zoom_polygrip")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "doll_zoom_polygrip_removed")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, new Rectangle(152, 187, 410, 248), "Polygrip01", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		if (m_game.m_inventory.FindItem("Polygrip01"))
		{
			HandlePolygrip01();
		}
	}

	public override void Reset()
	{
		base.Reset();
		m_game.m_game_data.SetState("Music", "");
	}

	private void HandlePolygrip01()
	{
		ChangeScene(1);
		m_triggers[0].m_enabled = false;
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Polygrip01":
			m_game.m_inventory.AskPickup(s_event);
			break;
		case "PickupPolygrip01":
			m_game.m_inventory.AddItem("Polygrip01");
			HandlePolygrip01();
			m_game.m_cursor.onOut();
			FadeFromScene(0);
			break;
		}
		base.HandleEvent(s_event);
	}
}
