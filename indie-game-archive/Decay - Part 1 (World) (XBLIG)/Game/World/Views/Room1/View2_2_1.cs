using Microsoft.Xna.Framework.Audio;

namespace Game.World.Views.Room1;

public class View2_2_1 : View
{
	private SoundEffect m_close_sound;

	public View2_2_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View2_2_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "badrumsskap_oppet")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "badrumsskap_oppet_tomt")));
		m_close_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/bathroom_skap_stangs");
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Clear()
	{
		m_close_sound = null;
		base.Clear();
	}

	public override void Setup()
	{
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		m_back_trigger.m_sound = m_close_sound;
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_pincett", "Pincett01", Trigger.TRIGGER_TYPE.USE_SMALL);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		if (m_game.m_inventory.FindItem("Pincett01"))
		{
			HandlePincett01();
		}
	}

	private void HandlePincett01()
	{
		ChangeScene(1);
		RemoveTrigger(m_triggers[0]);
		m_room.HandleEvent("View2_2.PlayGateSound");
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "PickupPincett01")
		{
			HandlePincett01();
			m_game.m_cursor.onOut();
			FadeFromScene(0);
		}
		base.HandleEvent(s_event);
	}
}
