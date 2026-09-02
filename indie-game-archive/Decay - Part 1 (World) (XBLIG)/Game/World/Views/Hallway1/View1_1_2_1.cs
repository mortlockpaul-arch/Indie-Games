using Microsoft.Xna.Framework.Audio;

namespace Game.World.Views.Hallway1;

internal class View1_1_2_1 : View
{
	private ViewTrigger m_VT_move_doll;

	private SoundEffect m_move_doll;

	public View1_1_2_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_1_2_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "doll_zoom")));
		m_move_doll = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/skrubb_docka_flyttas");
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
		m_use_text_fade = true;
	}

	public override void Clear()
	{
		m_move_doll = null;
		if (m_VT_move_doll != null)
		{
			m_VT_move_doll.Clear();
			m_VT_move_doll = null;
		}
		base.Clear();
	}

	public override void Setup()
	{
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		m_VT_move_doll = new ViewTrigger(m_game, this, m_room.GetView("View1_1_2_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		m_VT_move_doll.m_sound = m_move_doll;
		m_VT_move_doll.m_sound_vol = 0.1f;
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_doll_zoom", "View1_1_2_1.MoveDoll", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	public override void Reset()
	{
		base.Reset();
		m_game.m_game_data.SetState("Music", "");
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "View1_1_2_1.MoveDoll":
			m_game.m_hud.ShowAsk("This doll feels familiar in a strange way ...", "MOVE DOLL", "CANCEL", "View1_1_2_1.onMoveDoll", "", m_use_text_fade);
			break;
		case "View1_1_2_1.onMoveDoll":
			m_room.HandleEvent("View1_1_2.DollMoved");
			m_game.ActivateTrigger(m_VT_move_doll);
			m_game.m_hud.m_state = HUD.HUD_STATE.NONE;
			break;
		}
		base.HandleEvent(s_event);
	}
}
