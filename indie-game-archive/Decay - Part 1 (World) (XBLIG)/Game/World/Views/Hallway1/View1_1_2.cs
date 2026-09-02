using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Game.World.Views.Hallway1;

internal class View1_1_2 : View
{
	private const int STATE_DEFAULT = 0;

	private const int STATE_DOLL_MOVED = 1;

	private const int STATE_POLYGRIP_REMOVED = 2;

	private const int STATE_EMPTY = 3;

	private int m_state;

	private float m_light_timer;

	private float m_light_timeout = 0.5f;

	private SoundEffect m_show_doll;

	private SoundEffect m_tick_sound;

	private SoundEffect m_skrap_sound;

	private SoundEffectInstance m_skrap_sound_inst;

	public View1_1_2(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_1_2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "skrubb_dark")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "skrubb")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "polygrip_unremoved")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "polygrip_removed")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "skrubb_empty")));
		m_show_doll = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/skrubb_docka_syns");
		m_tick_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/korri_combinationlock_sound");
		m_skrap_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/skrubb_oljud");
		m_skrap_sound_inst = m_skrap_sound.CreateInstance();
		m_skrap_sound_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f;
		m_skrap_sound_inst.IsLooped = true;
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Clear()
	{
		m_show_doll = null;
		m_tick_sound = null;
		if (m_skrap_sound_inst != null)
		{
			m_skrap_sound_inst.Stop();
			m_skrap_sound_inst.Dispose();
			m_skrap_sound_inst = null;
		}
		m_skrap_sound = null;
		base.Clear();
	}

	public override void Setup()
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1_2_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_doll", trigger, Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1_2_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(283, 185, 363, 202), trigger, Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, Game.VIEW_RECT, "View1_1_2.onZoomDark", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_boxes", "View1_1_2.onBoxes", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(466, 125, 515, 360), "View1_1_2.onShovel", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		if (m_game.m_game_data.GetState("Hallway1.View1_1_2.State") != "")
		{
			m_state = int.Parse(m_game.m_game_data.GetState("Hallway1.View1_1_2.State"));
		}
		else
		{
			m_game.m_game_data.SetState("Hallway1.View1_1_2.State", m_state.ToString());
		}
		if (m_game.m_game_data.GetState("Hallway1.View1_1_2.Light") == "1")
		{
			HandleState();
		}
	}

	public override void Reset()
	{
		base.Reset();
		m_game.FadeOutMusic();
		m_game.m_game_data.SetState("Music", "");
		if (m_game.m_game_data.GetState("Hallway1.View1_1_2.SeenDoll") != "1")
		{
			m_skrap_sound_inst.Play();
		}
	}

	private void HandleState()
	{
		switch (m_state)
		{
		case 0:
			ChangeScene(1);
			m_triggers[0].m_enabled = true;
			m_triggers[1].m_enabled = false;
			m_triggers[2].m_enabled = false;
			if (m_game.m_game_data.GetState("Hallway1.View1_1_2.SeenDoll") != "1")
			{
				m_game.m_game_data.SetState("Hallway1.View1_1_2.SeenDoll", "1");
				m_game.PlaySound(m_show_doll, 0.5f);
			}
			break;
		case 1:
			ChangeScene(2);
			m_triggers[0].m_enabled = false;
			m_triggers[1].m_enabled = true;
			m_triggers[2].m_enabled = false;
			break;
		case 2:
			ChangeScene(3);
			m_triggers[0].m_enabled = false;
			m_triggers[1].m_enabled = false;
			m_triggers[2].m_enabled = false;
			break;
		case 3:
			ChangeScene(4);
			m_triggers[0].m_enabled = false;
			m_triggers[1].m_enabled = false;
			m_triggers[2].m_enabled = false;
			break;
		}
		m_triggers[3].m_enabled = true;
		m_triggers[4].m_enabled = true;
		m_game.m_cursor.onOut();
	}

	public override bool HandleUseEvent(string s_event)
	{
		switch (s_event)
		{
		case "Flashlight01":
			m_game.m_hud.ShowText("It's not working, it has no battery.", m_use_text_fade);
			return true;
		case "Flashlight02":
			if (m_game.m_game_data.GetState("Hallway1.View1_1_2.Light") != "1")
			{
				m_game.m_game_data.SetState("Hallway1.View1_1_2.Light", "1");
				m_light_timer = m_light_timeout;
				m_game.m_input_enabled = false;
				m_game.m_inventory_enabled = false;
				m_game.m_show_cursor = false;
			}
			return true;
		default:
			return base.HandleUseEvent(s_event);
		}
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "View1_1_2.LightOff":
			ChangeScene(0);
			m_triggers[0].m_enabled = false;
			m_triggers[1].m_enabled = false;
			m_triggers[2].m_enabled = true;
			m_triggers[3].m_enabled = false;
			m_triggers[4].m_enabled = false;
			m_game.m_game_data.SetState("Hallway1.View1_1_2.Light", "0");
			m_skrap_sound_inst.Stop();
			if (m_state == 2)
			{
				m_state = 3;
				m_game.m_game_data.SetState("Hallway1.View1_1_2.State", m_state.ToString());
				m_game.m_game_data.SetState("Room1.GateState", "3");
			}
			break;
		case "View1_1_2.DollMoved":
			m_state = 1;
			m_game.m_game_data.SetState("Hallway1.View1_1_2.State", m_state.ToString());
			HandleState();
			break;
		case "PickupPolygrip01":
			m_state = 2;
			m_game.m_game_data.SetState("Hallway1.View1_1_2.State", m_state.ToString());
			HandleState();
			m_game.m_game_data.SetState("Hallway1.View1_1.PlayFootsteps", "1");
			break;
		case "View1_1_2.onZoomDark":
			if (m_game.m_game_data.GetState("Hallway1.View1_1_2.SeenDoll") != "1")
			{
				m_game.m_hud.ShowText("It's too dark, I can't see anything. What's that terrible noise!?", m_use_text_fade);
			}
			else
			{
				m_game.m_hud.ShowText("It's too dark, I can't see anything.", m_use_text_fade);
			}
			break;
		case "View1_1_2.onBoxes":
			m_game.m_hud.ShowText("Nothing of interest ...", m_use_text_fade);
			break;
		case "View1_1_2.onShovel":
			m_game.m_hud.ShowText("Nothing of interest ...", m_use_text_fade);
			break;
		case "VolumeChanged":
			if (m_skrap_sound_inst != null)
			{
				m_skrap_sound_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f;
			}
			break;
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
		if (m_light_timer > 0f)
		{
			m_light_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_light_timer <= 0f)
			{
				m_light_timer = 0f;
				m_game.m_input_enabled = true;
				m_game.m_inventory_enabled = true;
				m_game.m_show_cursor = true;
				HandleState();
				m_game.PlaySound(m_tick_sound, 0.5f, 0f, -0.5f);
				m_skrap_sound_inst.Stop();
			}
		}
	}
}
