using Microsoft.Xna.Framework.Audio;

namespace Game.World.Views.Room1;

internal class View2_2 : View
{
	private SoundEffect m_open_sound;

	private SoundEffect m_gate_sound;

	private bool m_play_gate_sound;

	public View2_2(Game game, Area room)
		: base(game, room)
	{
		m_name = "View2_2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "badrumsskap")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "badrumsskap_gatetext")));
		m_open_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/bathroom_skap_oppnas");
		m_gate_sound = m_room.m_CL.LoadSound("World/Room3/Sound/rightroom_trycka_pa_tavla");
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Clear()
	{
		m_open_sound = null;
		m_gate_sound = null;
		base.Clear();
	}

	public override void Setup()
	{
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		ViewTrigger viewTrigger = new ViewTrigger(m_game, this, m_room.GetView("View2_2_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		viewTrigger.m_sound = m_open_sound;
		CursorTrigger item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_spegel_zoom", viewTrigger, Trigger.TRIGGER_TYPE.USE);
		m_triggers.Add(item);
		if (m_game.m_game_data.GetState("Room1.GateState") == "1")
		{
			HandlePincett01();
		}
	}

	public override void Reset()
	{
		base.Reset();
		if (m_play_gate_sound)
		{
			m_play_gate_sound = false;
			m_game.PlaySound(m_gate_sound, 0.5f);
		}
	}

	private void HandlePincett01()
	{
		ChangeScene(1);
		RemoveTrigger(m_triggers[0]);
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "PickupPincett01":
			m_game.m_game_data.SetState("Room1.GateState", "1");
			HandlePincett01();
			break;
		case "View2_2.PlayGateSound":
			m_play_gate_sound = true;
			break;
		}
		base.HandleEvent(s_event);
	}
}
