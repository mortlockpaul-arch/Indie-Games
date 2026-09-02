using Microsoft.Xna.Framework;

namespace Game.World.Views.Room3;

internal class View2_2_1 : View
{
	public View2_2_1(Game game, Area room)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(game, room);
		m_name = "View2_2_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view2_2_1_empty")));
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
		m_items.Add(new ViewItem("Flashlight01", m_game, m_room.m_CL, m_room.m_content_path + "view2_2_1_items", new Rectangle(245, 193, 105, 32)));
		m_items.Add(new ViewItem("Note01", m_game, m_room.m_CL, m_room.m_content_path + "view2_2_1_items", new Rectangle(245, 100, 55, 93)));
	}

	public override void Setup()
	{
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_flashlight", "Flashlight01", Trigger.TRIGGER_TYPE.USE_SMALL);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_letter", "Note01", Trigger.TRIGGER_TYPE.USE_SMALL);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		if (m_game.m_inventory.FindItem("Flashlight01"))
		{
			HandleFlashlightPickup();
		}
		if (m_game.m_inventory.FindItem("Note01"))
		{
			HandleNotePickup();
		}
	}

	private void HandleFlashlightPickup()
	{
		RemoveItem("Flashlight01");
		m_triggers[0].m_enabled = false;
	}

	private void HandleNotePickup()
	{
		RemoveItem("Note01");
		m_triggers[1].m_enabled = false;
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Flashlight01":
		case "Note01":
			m_game.m_inventory.AskPickup(s_event);
			break;
		case "PickupFlashlight01":
			m_game.m_inventory.AddItem("Flashlight01");
			m_game.m_cursor.onOut();
			HandleFlashlightPickup();
			if (m_items[0].m_state == ViewItem.VIEWITEM_STATE.REMOVED && m_items[1].m_state == ViewItem.VIEWITEM_STATE.REMOVED)
			{
				m_game.HandleEvent("Room3.Completed");
			}
			break;
		case "PickupNote01":
			m_game.m_inventory.AddItem("Note01");
			m_game.m_cursor.onOut();
			HandleNotePickup();
			if (m_items[0].m_state == ViewItem.VIEWITEM_STATE.REMOVED && m_items[1].m_state == ViewItem.VIEWITEM_STATE.REMOVED)
			{
				m_game.HandleEvent("Room3.Completed");
			}
			break;
		}
		base.HandleEvent(s_event);
	}
}
