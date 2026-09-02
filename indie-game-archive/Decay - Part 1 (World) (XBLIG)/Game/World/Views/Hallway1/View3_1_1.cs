using Microsoft.Xna.Framework;

namespace Game.World.Views.Hallway1;

internal class View3_1_1 : View
{
	public View3_1_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View3_1_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "Left room/note_bloodspurting_zoom")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "Left room/note_bloodspurting_zoom2")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Setup()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View3_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, new Rectangle(241, 144, 373, 216), "Note02", Trigger.TRIGGER_TYPE.USE_SMALL);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		if (m_game.m_inventory.FindItem("Note02"))
		{
			HandleNote02();
		}
	}

	private void HandleNote02()
	{
		ChangeScene(1);
		m_triggers[0].m_enabled = false;
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Note02":
			m_game.m_inventory.AskPickup(s_event);
			break;
		case "PickupNote02":
			m_game.m_inventory.AddItem("Note02");
			HandleNote02();
			m_game.m_cursor.onOut();
			m_game.PlaySound(m_game.m_door_open, 0.5f, -1f, -1f);
			break;
		}
		base.HandleEvent(s_event);
	}
}
