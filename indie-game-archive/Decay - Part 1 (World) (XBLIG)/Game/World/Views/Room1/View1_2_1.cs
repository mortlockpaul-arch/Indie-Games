using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Game.World.Views.Room1;

public class View1_2_1 : View
{
	private SoundEffect m_clear_sound;

	private SoundEffect m_unlock_sound;

	public View1_2_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_2_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "lock_seethrough0")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "lock_seethrough2")));
		m_clear_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/bathroom_rensa_nyckelhal");
		m_unlock_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/door_unlocks");
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Clear()
	{
		m_clear_sound = null;
		m_unlock_sound = null;
		base.Clear();
	}

	public override void Setup()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, new Rectangle(100, 0, 540, (Game.VIEW_RECT.Height - 200) / 2), "KeyholeStuck", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		if (m_game.m_game_data.GetState("Room1.Keyhole") == "Clear")
		{
			HandlePincett01();
		}
	}

	private void HandlePincett01()
	{
		ChangeScene(1);
		RemoveTrigger(m_triggers[0]);
	}

	public override bool HandleUseEvent(string s_event)
	{
		switch (s_event)
		{
		case "Pincett01":
			if (m_game.m_game_data.GetState("Room1.Keyhole") != "Clear")
			{
				m_game.m_game_data.SetState("Room1.Keyhole", "Clear");
				HandlePincett01();
				m_game.PlaySound(m_clear_sound, 0.5f);
				m_game.m_cursor.onOut();
				FadeFromScene(0);
				return true;
			}
			break;
		case "Key01":
			if (m_game.m_game_data.GetState("Room1.Keyhole") == "Clear")
			{
				m_game.m_game_data.SetState("Room1.Door", "Unlocked");
				m_room.GetView("View1").HandleUseEvent(s_event);
				m_game.m_inventory.RemoveItem("Key01");
				m_back_trigger.m_next_view = m_room.GetView("View1");
				m_game.PlaySound(m_unlock_sound, 0.5f);
				onBack();
			}
			else
			{
				m_game.m_hud.ShowText("Something seems to be stuck in there ...", m_use_text_fade);
			}
			return true;
		}
		return base.HandleUseEvent(s_event);
	}

	public override void HandleEvent(string s_event)
	{
		base.HandleEvent(s_event);
	}
}
